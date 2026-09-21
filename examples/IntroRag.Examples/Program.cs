using IntroRag.Examples;

// Menu des exemples (chaque entrée = un module/exercice porté depuis les notebooks).
var items = new (string Key, string Title, Func<Task> Run)[]
{
    ("1",  "Module 1 — Parler à un LLM local",         Module01.Run),
    ("2",  "Module 2 — Prompts, rôles & température",   Module02.Run),
    ("3",  "Module 3 — Embeddings & recherche (Qdrant)",Module03.Run),
    ("4",  "Module 4 — Le RAG (sans vs avec)",          Module04.Run),
    ("5",  "Module 5 — Ingestion de documents",         Module05.Run),
    ("6",  "Module 6 — Mini-projet assistant RAG",      Module06.Run),
    ("e1", "Exercice 1 — Températures (solution)",      ExSol01.Run),
    ("e2", "Exercice 2 — Persona (solution)",           ExSol02.Run),
    ("e3", "Exercice 3 — Recherche (solution)",         ExSol03.Run),
    ("e4", "Exercice 4 — Fallback RAG (solution)",      ExSol04.Run),
    ("e5", "Exercice 5 — Idempotence (solution)",       ExSol05.Run),
    ("e6", "Exercice 6 — Fait métier + RAG (solution)", ExSol06.Run),
};

Console.WriteLine("=== Exemples Intro RAG .NET (Semantic Kernel + Ollama + Qdrant) ===\n");
foreach (var it in items) Console.WriteLine($"  {it.Key,-3} {it.Title}");
Console.WriteLine("  all  Tout exécuter");
Console.Write("\nChoix : ");

// Support d'un argument en ligne de commande : dotnet run -- 3
var choice = (args.Length > 0 ? args[0] : Console.ReadLine())?.Trim().ToLowerInvariant() ?? "";

async Task RunOne((string Key, string Title, Func<Task> Run) it)
{
    Console.WriteLine($"\n===== {it.Title} =====");
    try { await it.Run(); }
    catch (Exception ex) { Console.WriteLine($"[Erreur] {ex.Message}\n(Vérifie que Ollama + Qdrant tournent : docker compose up -d)"); }
}

if (choice == "all")
{
    foreach (var it in items) await RunOne(it);
}
else
{
    var match = Array.Find(items, x => x.Key == choice);
    if (match.Run is not null) await RunOne(match);
    else Console.WriteLine("Choix inconnu.");
}
