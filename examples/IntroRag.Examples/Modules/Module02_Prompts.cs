namespace IntroRag.Examples;

// Module 2 — Prompts, rôles (system/user) & température.
public static class Module02
{
    public static async Task Run()
    {
        var kernel = Sk.Kernel();
        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory("Tu es un assistant support technique. Réponds en une phrase, sans blabla.");
        history.AddUserMessage("Le Wi-Fi de la salle 3 est instable, que faire ?");

        var settings = new OllamaPromptExecutionSettings { Temperature = 0f };
        var rep = await chat.GetChatMessageContentAsync(history, settings, kernel);
        Console.WriteLine(rep.Content);
    }
}
