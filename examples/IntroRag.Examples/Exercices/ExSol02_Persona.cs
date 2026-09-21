namespace IntroRag.Examples;

// Exercice 2 (solution) — support N1 laconique : une phrase.
public static class ExSol02
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var h = new ChatHistory("Support N1. Une phrase maximum, pas de politesse superflue.");
        h.AddUserMessage("Comment redemarrer ma box internet ?");
        Console.WriteLine((await chat.GetChatMessageContentAsync(h, new OllamaPromptExecutionSettings { Temperature = 0f }, kernel)).Content);
    }
}
