namespace IntroRag.Examples;

// Exercice 1 (solution) — même question à Temperature 0 puis 1 : comparer la stabilité.
public static class ExSol01
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var h = new ChatHistory();
        h.AddUserMessage("Donne un synonyme de rapide.");
        Console.WriteLine("T=0 : " + (await chat.GetChatMessageContentAsync(h, new OllamaPromptExecutionSettings { Temperature = 0f }, kernel)).Content);
        Console.WriteLine("T=1 : " + (await chat.GetChatMessageContentAsync(h, new OllamaPromptExecutionSettings { Temperature = 1f }, kernel)).Content);
    }
}
