using Qdrant.Client.Grpc;
using System.Security.Cryptography;
using System.Text;

namespace IntroRag.Examples;

/// <summary>Configuration locale (variables d'environnement + valeurs par défaut).</summary>
static class Config
{
    public static Uri Endpoint    => new(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT")    ?? "http://localhost:11434");
    public static string RagModel  => Environment.GetEnvironmentVariable("OLLAMA_RAG_MODEL")   ?? "gemma3:4b";
    public static string EmbedModel=> Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text";
    public static string QdrantHost=> Environment.GetEnvironmentVariable("QDRANT_HOST")        ?? "localhost";
}

/// <summary>Fabriques Semantic Kernel réutilisées par les exemples.</summary>
static class Sk
{
    public static Kernel Kernel() =>
        global::Microsoft.SemanticKernel.Kernel.CreateBuilder()
            .AddOllamaChatCompletion(Config.RagModel, Config.Endpoint).Build();

    public static IEmbeddingGenerator<string, Embedding<float>> Embeddings() =>
        global::Microsoft.SemanticKernel.Kernel.CreateBuilder()
            .AddOllamaEmbeddingGenerator(Config.EmbedModel, Config.Endpoint)
            .Build().GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

    public static QdrantVectorStore Store() => new(new QdrantClient(Config.QdrantHost), ownsClient: true);
}

/// <summary>ID déterministe (hash) => ingestion idempotente.</summary>
static class Ids
{
    public static ulong Of(string s) => BitConverter.ToUInt64(MD5.HashData(Encoding.UTF8.GetBytes(s)), 0);
}

/// <summary>Enregistrement Qdrant partagé par les exemples (768 = dim. de nomic-embed-text).</summary>
public sealed class KbDoc
{
    [VectorStoreKey] public ulong Id { get; set; }
    [VectorStoreData] public string Text { get; set; } = "";
    [VectorStoreData(IsIndexed = true)] public string Source { get; set; } = "";
    [VectorStoreData(IsIndexed = true)] public string categorie { get; set; } = "";
    [VectorStoreVector(768, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; }
}

/// <summary>Localise data/support_kb en remontant depuis le dossier d'exécution.</summary>
static class Paths
{
    public static string KbDir
    {
        get
        {
            var dir = AppContext.BaseDirectory;
            for (int i = 0; i < 8; i++)
            {
                var candidate = Path.Combine(dir, "data", "support_kb");
                if (Directory.Exists(candidate)) return candidate;
                dir = Path.GetFullPath(Path.Combine(dir, ".."));
            }
            return Path.Combine("data", "support_kb");
        }
    }
}
