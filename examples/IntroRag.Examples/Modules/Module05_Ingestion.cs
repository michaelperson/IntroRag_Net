namespace IntroRag.Examples;

// Module 5 — Ingestion de fichiers Markdown (découpage + ID déterministe).
public static class Module05
{
    public static async Task Run()
    {
        var embed = Sk.Embeddings();
        var col = Sk.Store().GetCollection<ulong, KbDoc>("support_kb");
        await col.EnsureCollectionExistsAsync();

        static IEnumerable<string> Chunk(string t, int max = 600) =>
            t.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
             .Select(p => p.Length <= max ? p.Trim() : p[..max].Trim())
             .Where(p => p.Length > 0);

        var dir = Paths.KbDir;
        int n = 0;
        foreach (var file in Directory.GetFiles(dir, "*.md"))
        {
            var source = Path.GetFileNameWithoutExtension(file);
            foreach (var chunk in Chunk(File.ReadAllText(file)))
            {
                var v = (await embed.GenerateAsync(chunk)).Vector;
                await col.UpsertAsync(new KbDoc { Id = Ids.Of(chunk), Text = chunk, Source = source, Vector = v });
                n++;
            }
        }
        Console.WriteLine($"{n} chunks indexés depuis {dir}");
    }
}
