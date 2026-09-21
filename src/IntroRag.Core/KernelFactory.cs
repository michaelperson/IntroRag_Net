using Microsoft.Extensions.AI;              // IEmbeddingGenerator, Embedding<float>
using Microsoft.SemanticKernel;

namespace IntroRag.Core;

/// <summary>Fabrique centralisée : un seul endroit qui lit la configuration locale.</summary>
public static class KernelFactory
{
    public static string Endpoint   => Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT")   ?? "http://localhost:11434";
    public static string RagModel   => Environment.GetEnvironmentVariable("OLLAMA_RAG_MODEL")  ?? "gemma3:4b";
    public static string EmbedModel => Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL")?? "nomic-embed-text";
    public static string QdrantHost => Environment.GetEnvironmentVariable("QDRANT_HOST")       ?? "localhost";

    public static Kernel CreateKernel()
        => Kernel.CreateBuilder().AddOllamaChatCompletion(RagModel, new Uri(Endpoint)).Build();

    /// <summary>
    /// Générateur d'embeddings MODERNE : Microsoft.Extensions.AI.IEmbeddingGenerator
    /// (remplace l'obsolète ITextEmbeddingGenerationService).
    /// </summary>
    public static IEmbeddingGenerator<string, Embedding<float>> CreateEmbeddingGenerator()
    {
        var builder = Kernel.CreateBuilder();
        builder.AddOllamaEmbeddingGenerator(EmbedModel, new Uri(Endpoint));
        return builder.Build().GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
    }
}
