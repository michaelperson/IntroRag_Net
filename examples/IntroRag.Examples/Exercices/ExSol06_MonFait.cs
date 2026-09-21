namespace IntroRag.Examples;

// Exercice 6 (solution) — ajouter un fait métier (VPN) et l'interroger en RAG.
public static class ExSol06
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var embed = Sk.Embeddings();
        var col = Sk.Store().GetCollection<ulong, KbDoc>("ex6");
        await col.EnsureCollectionExistsAsync();

        var fait = "Le VPN se configure via le portail interne vpn.cognitic.local avec vos identifiants AD.";
        await col.UpsertAsync(new KbDoc { Id = Ids.Of(fait), Text = fait, Vector = (await embed.GenerateAsync(fait)).Vector });

        var qv = (await embed.GenerateAsync("comment configurer le VPN ?")).Vector;
        var passages = new List<string>();
        await foreach (var r in col.SearchAsync(qv, top: 2)) passages.Add(r.Record.Text);

        var rep = await kernel.InvokePromptAsync(
            "Reponds depuis le contexte.\nContexte: {{$c}}\nQuestion: {{$q}}",
            new KernelArguments { ["c"] = string.Join("\n", passages), ["q"] = "Comment configurer le VPN ?" });
        Console.WriteLine(rep);
    }
}
