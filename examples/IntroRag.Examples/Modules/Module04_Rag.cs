namespace IntroRag.Examples;

// Module 4 — Le RAG : récupérer (Qdrant) + générer. SANS vs AVEC.
public static class Module04
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var embed = Sk.Embeddings();
        var col = Sk.Store().GetCollection<ulong, KbDoc>("demo_kb");
        await col.EnsureCollectionExistsAsync();

        async Task Add(string t, string s)
        {
            var v = (await embed.GenerateAsync(t)).Vector;
            await col.UpsertAsync(new KbDoc { Id = Ids.Of(t), Text = t, Source = s, Vector = v });
        }
        await Add("Wi-Fi instable : redemarrer le point d'acces, verifier le canal 5GHz.", "reseau");

        async Task<string> Retrieve(string q, int k = 3)
        {
            var qv = (await embed.GenerateAsync(q)).Vector;
            var passages = new List<string>();
            await foreach (var r in col.SearchAsync(qv, top: k)) passages.Add(r.Record.Text);
            return passages.Count == 0 ? "(aucun contexte)" : string.Join("\n", passages);
        }

        var question = "Que faire si le wifi est instable ?";

        var sans = await kernel.InvokePromptAsync("{{$q}}", new KernelArguments { ["q"] = question });
        Console.WriteLine("--- SANS RAG ---\n" + sans + "\n");

        var ctx = await Retrieve(question);
        var avec = await kernel.InvokePromptAsync(
            "Réponds UNIQUEMENT à partir du contexte, sinon dis que tu ne sais pas.\nContexte:\n{{$c}}\n\nQuestion: {{$q}}",
            new KernelArguments { ["c"] = ctx, ["q"] = question });
        Console.WriteLine("--- AVEC RAG ---\n" + avec);
    }
}
