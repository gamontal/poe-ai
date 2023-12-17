using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SnapPoet.Models.Configurations;
using System.Text;

namespace SnapPoet.Clients;

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

    public async Task<string?> GetCompletion(
		string prompt,
		string model = "text-davinci-003",
		float temperature = 0,
		int maxTokens = 150)
    {
		var completionRequest = new StringContent(
			JsonConvert.SerializeObject(new OpenAiCompletionRequest
			{
				Model = model,
				Prompt = prompt,
				Temperature = temperature,
				MaxTokens = maxTokens
			}),
			Encoding.UTF8,
			"application/json");

		var completionResponse = await _httpClient.PostAsync("completions", completionRequest);

		completionResponse.EnsureSuccessStatusCode();

		var completionResponseContent = await completionResponse.Content.ReadAsStringAsync();

		var completion = JsonConvert.DeserializeObject<OpenAiCompletionResponse>(completionResponseContent);
		var completionText = completion?.Choices.FirstOrDefault()?.Text;

		return completionText;
    }

	public class OpenAiCompletionRequest
	{
		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("prompt")]
		public string Prompt { get; set; }

		[JsonProperty("temperature")]
		public float Temperature { get; set; }

		[JsonProperty("max_tokens")]
		public int MaxTokens { get; set; }
	}

	public class OpenAiCompletionResponseChoice
	{
		public string Text { get; set; }
	}

	public class OpenAiCompletionResponse
	{
		public List<OpenAiCompletionResponseChoice> Choices { get; set; }
	}
}