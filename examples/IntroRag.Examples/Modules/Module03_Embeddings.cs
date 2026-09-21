namespace IntroRag.Examples;

// Module 3 — Embeddings (IEmbeddingGenerator) & recherche sémantique (Qdrant).
public static class Module03
{
    public static async Task Run()
    {
        var embed = Sk.Embeddings();
        var store = Sk.Store();
        var col = store.GetCollection<ulong, KbDoc>("demo_kb");
        await col.EnsureCollectionExistsAsync();

        async Task Add(string text, string source, string categorie)
        {
            var v = (await embed.GenerateAsync(text)).Vector;

            await col.UpsertAsync(new KbDoc { Id = Ids.Of(text), Text = text, Source = source, categorie= categorie, Vector = v });
        }

        await Add("Wi-Fi instable : redemarrer le point d'acces.","Networks4Beginners.pdf", "reseau");
        await Add("Mot de passe oublie : self-service de reset.","GuideDesurvie.md", "compte");
        await Add("Application qui plante : vider le cache.","Copie_website_microsft.txt" ,"logiciel");

        // La question ne contient aucun mot-clé exact -> la bonne procédure remonte quand même.
        var qv = (await embed.GenerateAsync("mon internet sans fil rame")).Vector;
        await foreach (var r in col.SearchAsync(qv, top: 1))
            Console.WriteLine($"[{r.Score:0.000}] {r.Record.Text}");
    }
}
