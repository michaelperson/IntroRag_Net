# IntroRag.Examples — tous les exemples des notebooks, en 1 projet

Chaque **module** et chaque **solution d'exercice** des notebooks Intro RAG a été porté
en code C# compilable, regroupé dans un **projet console unique** piloté par un **menu**.

## Ouvrir la solution
Ouvrir **`IntroRag.Examples.slnx`** (format solution moderne, VS 2022 17.10+ / `dotnet` 9+).
Elle contient : `IntroRag.Core`, `IntroRag.Console` et `IntroRag.Examples`.

## Prérequis
```powershell
cd infrastructure ; docker compose up -d   # Ollama + Qdrant
```

## Lancer
```powershell
# Menu interactif
dotnet run --project examples/IntroRag.Examples

# Directement un exemple (argument = clé du menu)
dotnet run --project examples/IntroRag.Examples -- 4     # Module 4 (RAG)
dotnet run --project examples/IntroRag.Examples -- e3    # Exercice 3 (solution)
dotnet run --project examples/IntroRag.Examples -- all   # tout enchaîner
```

## Contenu (clé du menu → exemple)
| Clé | Exemple |
|-----|---------|
| 1 | Module 1 — Parler à un LLM local |
| 2 | Module 2 — Prompts, rôles & température |
| 3 | Module 3 — Embeddings & recherche (Qdrant) |
| 4 | Module 4 — Le RAG (sans vs avec) |
| 5 | Module 5 — Ingestion de documents |
| 6 | Module 6 — Mini-projet assistant RAG |
| e1…e6 | Solutions des exercices 1 à 6 |

> Chaque exemple est **autonome** (pas de dépendance à IntroRag.Core) et reprend fidèlement
> le code de la cellule du notebook correspondant. API 100 % moderne : `IEmbeddingGenerator` + Qdrant.
