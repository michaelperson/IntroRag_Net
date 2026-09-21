namespace IntroRag.Examples;

// Exercice 3 (solution) — recherche par le sens : "boisson chaude" -> café/thé avant vélo.
public static class ExSol03
{
    public static async Task Run()
    {
        var embed = Sk.Embeddings();
        var col = Sk.Store().GetCollection<ulong, KbDoc>("ex3");
        await col.EnsureCollectionExistsAsync();

        async Task Add(string t)
        {
            var v = (await embed.GenerateAsync(t)).Vector;
            await col.UpsertAsync(new KbDoc { Id = Ids.Of(t), Text = t, Vector = v });
        }
        await Add("Le cafe est une boisson chaude a base de grains torrefies.");
        await Add("Le the est une infusion chaude de feuilles.");
        await Add("Le velo est un moyen de transport a deux roues.");

        var qv = (await embed.GenerateAsync("boisson chaude")).Vector;
        await foreach (var r in col.SearchAsync(qv, top: 3))
            Console.WriteLine($"[{r.Score:0.000}] {r.Record.Text}");
    }
}
