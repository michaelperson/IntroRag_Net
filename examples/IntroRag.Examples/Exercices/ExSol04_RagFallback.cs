namespace IntroRag.Examples;

// Exercice 4 (solution) — le fallback : question hors contexte -> le modèle refuse.
public static class ExSol04
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var ctx = "Procedure reseau: redemarrer le point d'acces.";
        var r = await kernel.InvokePromptAsync(
            "Reponds UNIQUEMENT depuis le contexte, sinon dis \"Je ne sais pas\".\nContexte: {{$c}}\nQuestion: {{$q}}",
            new KernelArguments { ["c"] = ctx, ["q"] = "Quel temps fait-il a Paris ?" });
        Console.WriteLine(r);
    }
}
