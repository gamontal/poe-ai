using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PoeThePoet.Models.Configurations;
using System.Text;

namespace PoeThePoet.Clients;

public class OpenAIClient
{
	private readonly OpenAIConfig _openAiConfig;
	private readonly HttpClient _httpClient;

	public OpenAIClient(
		IOptions<OpenAIConfig> openAiConfig,
		HttpClient httpClient)
	{
		_openAiConfig = openAiConfig.Value;

		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri(_openAiConfig.Endpoint);
		_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiConfig.Key}");
	}

	public async Task<string?> GetTextCompletion(
		string metaPrompt,
		string prompt,
		string model = "gpt-4",
		float temperature = 0.7f,
		int maxTokens = 300)
	{
		var messages = new List<ChatMessage>
		{
			new()
			{
				Role = "system",
				Content = metaPrompt
			},
			new()
			{
				Role = "user",
				Content = prompt
			}
		};

		var completionRequest = JsonConvert.SerializeObject(new ChatCompletionRequest
		{
			Model = model,
			Messages = messages,
			Temperature = temperature,
			MaxTokens = maxTokens
		});

		var completionResponse = await _httpClient.PostAsync(
			"chat/completions",
			new StringContent(
				completionRequest,
				Encoding.UTF8,
				"application/json"));

		completionResponse.EnsureSuccessStatusCode();

		var completionResponseContent = await completionResponse.Content.ReadAsStringAsync();

		var completion = JsonConvert.DeserializeObject<ChatCompletionResponse>(completionResponseContent);
		var completionText = completion?.Choices.FirstOrDefault()?.Message.Content;

		return completionText;
	}

	public class ChatCompletionRequest
	{
		[JsonProperty("model")]
		public string Model { get; set; }
		[JsonProperty("messages")]
		public IList<ChatMessage> Messages { get; set; }
		[JsonProperty("temperature")]
		public float Temperature { get; set; }
		[JsonProperty("max_tokens")]
		public int MaxTokens { get; set; }
	}

	public class ChatCompletionResponse
	{
		public List<ChatCompletionResponseChoice> Choices { get; set; }
	}

	public class ChatCompletionResponseChoice
	{
		public ChatMessage Message { get; set; }
	}

	public class ChatMessage
	{
		[JsonProperty("role")]
		public string Role { get; set; }
		[JsonProperty("content")]
		public string Content { get; set; }
	}
}