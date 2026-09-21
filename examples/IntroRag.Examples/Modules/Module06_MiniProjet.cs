namespace IntroRag.Examples;

// Module 6 — Mini-projet : assistant support RAG (réponse ancrée + sources, refus hors base).
public static class Module06
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var embed = Sk.Embeddings();
        var col = Sk.Store().GetCollection<ulong, KbDoc>("assistant_kb");
        await col.EnsureCollectionExistsAsync();

        // 1) Ingestion
        foreach (var file in Directory.GetFiles(Paths.KbDir, "*.md"))
        {
            var src = Path.GetFileNameWithoutExtension(file);
            foreach (var para in File.ReadAllText(file).Split("\n\n", StringSplitOptions.RemoveEmptyEntries))
            {
                var v = (await embed.GenerateAsync(para.Trim())).Vector;
                await col.UpsertAsync(new KbDoc { Id = Ids.Of(para), Text = para.Trim(), Source = src, Vector = v });
            }
        }

        // 2) Poser une question -> réponse + sources
        async Task Ask(string q)
        {
            var qv = (await embed.GenerateAsync(q)).Vector;
            var passages = new List<string>();
            var sources = new HashSet<string>();
            await foreach (var r in col.SearchAsync(qv, top: 4))
            {
                passages.Add(r.Record.Text);
                if (!string.IsNullOrWhiteSpace(r.Record.Source)) sources.Add(r.Record.Source);
            }
            var ctx = passages.Count == 0 ? "(aucun contexte)" : string.Join("\n", passages);
            var rep = await kernel.InvokePromptAsync(
                "Réponds UNIQUEMENT depuis le contexte, sinon dis \"Je ne trouve pas cette information\".\nContexte:\n{{$c}}\nQuestion: {{$q}}",
                new KernelArguments { ["c"] = ctx, ["q"] = q });
            Console.WriteLine($"Q: {q}\nR: {rep}\nSources: {string.Join(", ", sources)}\n");
        }

        await Ask("Mon wifi est instable");
        await Ask("J'ai oublie mon mot de passe");
        await Ask("Quelle est la capitale du Japon ?");   // hors base -> refus attendu
    }
}
