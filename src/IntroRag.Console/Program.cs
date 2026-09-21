using IntroRag.Core;

// =====================================================================
// Démo intro RAG bout-en-bout (la "démo qui tourne" du support) :
//   1) ingérer la base de connaissances
//   2) poser des questions -> réponse ancrée + sources
// Prérequis : docker compose up -d (Ollama + Qdrant).
// =====================================================================

var kernel = KernelFactory.CreateKernel();
var embeddings = KernelFactory.CreateEmbeddingGenerator();
var rag = new RagService(kernel, embeddings, KernelFactory.QdrantHost);

// data/support_kb par rapport à la racine de la solution
var kbDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data", "support_kb");
kbDir = Path.GetFullPath(kbDir);

Console.WriteLine($"Ingestion de {kbDir} ...");
var n = await rag.IngestFolderAsync(kbDir);
Console.WriteLine($"{n} chunks indexés.\n");

string[] questions =
{
    "Mon Wi-Fi est instable, que faire ?",
    "J'ai oublié mon mot de passe.Je voudrais le réinitialiser",
    "Quelle est la capitale du Japon ?",   // hors base -> doit refuser
};

foreach (var q in questions)
{
    var (answer, sources) = await rag.AskAsync(q);
    Console.WriteLine($"Q: {q}");
    Console.WriteLine($"R: {answer.Trim()}");
    Console.WriteLine(sources.Count > 0 ? $"Sources: {string.Join(", ", sources)}\n" : "Sources: (aucune)\n");
}
