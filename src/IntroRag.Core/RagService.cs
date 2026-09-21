using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.AI;              // IEmbeddingGenerator, Embedding<float>
using Microsoft.Extensions.VectorData;      // VectorStore attributs + collection
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

namespace IntroRag.Core;

/// <summary>Un enregistrement stocké dans Qdrant (clé + données + vecteur).</summary>
public sealed class KbChunk
{
    [VectorStoreKey] public ulong Id { get; set; }
    [VectorStoreData] public string Text { get; set; } = "";
    [VectorStoreData(IsIndexed = true)] public string Source { get; set; } = "";
    // nomic-embed-text produit des vecteurs de dimension 768.
    [VectorStoreVector(768, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}

/// <summary>
/// RAG en 3 gestes : INGÉRER, RÉCUPÉRER, GÉNÉRER — stack 100 % moderne :
/// embeddings Microsoft.Extensions.AI (IEmbeddingGenerator) + Qdrant (VectorData).
/// </summary>
public sealed class RagService
{
    private readonly Kernel _kernel;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddings;
    private readonly VectorStoreCollection<ulong, KbChunk> _collection;

    public RagService(Kernel kernel, IEmbeddingGenerator<string, Embedding<float>> embeddings, string qdrantHost)
    {
        _kernel = kernel;
        _embeddings = embeddings;
        var store = new QdrantVectorStore(new QdrantClient(qdrantHost), ownsClient: true);
        _collection = store.GetCollection<ulong, KbChunk>("intro_rag_kb");
    }

    // ID déterministe (8 premiers octets du MD5) => ré-ingestion idempotente.
    private static ulong DeterministicId(string s) => BitConverter.ToUInt64(MD5.HashData(Encoding.UTF8.GetBytes(s)), 0);

    private static IEnumerable<string> Chunk(string text, int max = 600)
        => text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
               .Select(p => p.Length <= max ? p.Trim() : p[..max].Trim())
               .Where(p => p.Length > 0);

    public async Task<int> IngestFolderAsync(string folder)
    {
        await _collection.EnsureCollectionExistsAsync();
        int n = 0;
        foreach (var file in Directory.GetFiles(folder, "*.md"))
        {
            var source = Path.GetFileNameWithoutExtension(file);
            foreach (var chunk in Chunk(File.ReadAllText(file)))
            {
                var vector = (await _embeddings.GenerateAsync(chunk)).Vector;
                await _collection.UpsertAsync(new KbChunk
                {
                    Id = DeterministicId(chunk), Text = chunk, Source = source, Embedding = vector
                });
                n++;
            }
        }
        return n;
    }

    public async Task<(string Answer, IReadOnlyList<string> Sources)> AskAsync(string question, int k = 3)
    {
        var queryVector = (await _embeddings.GenerateAsync(question)).Vector;
        var passages = new List<string>();
        var sources = new List<string>();
        await foreach (var result in _collection.SearchAsync(queryVector, top: k))
        {
            passages.Add(result.Record.Text);
            if (!string.IsNullOrWhiteSpace(result.Record.Source)) sources.Add(result.Record.Source);
        }

        var contexte = passages.Count == 0 ? "(aucun contexte)" : string.Join("\n", passages);
        var reponse = await _kernel.InvokePromptAsync(
            "Tu es un assistant support. Réponds UNIQUEMENT à partir du contexte ci-dessous. " +
            "Si l'information n'y est pas, réponds : \"Je ne trouve pas cette information dans la base.\"\n\n" +
            "Contexte:\n{{$ctx}}\n\nQuestion: {{$q}}",
            new KernelArguments { ["ctx"] = contexte, ["q"] = question });

        return (reponse.ToString(), sources.Distinct().ToList());
    }
}
