namespace IntroRag.Examples;

// Module 1 — Parler à un LLM local (le Kernel) + démo d'hallucination.
public static class Module01
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();

        //var r = await kernel.InvokePromptAsync("Explique le RAG en une phrase simple.");
        //Console.WriteLine(r);

        // Question sur une donnée interne fictive : le modèle NE PEUT PAS savoir -> il invente.
        var halluc = await kernel.InvokePromptAsync(
            "Quel est le numero du ticket concernant la panne wifi de la salle 3 chez Cognitic ?");
        Console.WriteLine("\n[Hallucination probable]\n" + halluc);
    }
}
