namespace PoeThePoet.Clients.Interfaces;

public interface IOpenAIClient
{
	Task<string?> GetTextCompletion(string metaPrompt, string prompt, string model = "gpt-4", float temperature = 0.7f, int maxTokens = 300);
}
