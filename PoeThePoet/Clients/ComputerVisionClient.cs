using PoeThePoet.Clients.Interfaces;
using PoeThePoet.Models.Configurations;

namespace PoeThePoet.Clients;

public class ComputerVisionClient : IComputerVisionClient
{
	private readonly AzureAIConfig _azureAiConfig;
	private readonly HttpClient _httpClient;

	public ComputerVisionClient(
		IOptions<AzureAIConfig> azureAiConfig,
		HttpClient httpClient)
	{
		_azureAiConfig = azureAiConfig.Value;

		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri(_azureAiConfig.ComputerVisionEndpoint);
		_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _azureAiConfig.ComputerVisionKey);
	}

	public async Task<DenseCaptions?> GetImageDenseCaptions(MemoryStream imageStream)
	{
		var byteContent = new ByteArrayContent(imageStream.ToArray());
		byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

		var response = await _httpClient.PostAsync(
			$"computervision/imageanalysis:analyze?api-version={_azureAiConfig.ComputerVisionApiVersion}&features=denseCaptions",
			byteContent);

		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<DenseCaptions>(responseContent);
	}

	public class DenseCaptions
	{
		public DenseCaptionsResult DenseCaptionsResult { get; set; }
		public string ModelVersion { get; set; }
	}

	public class Value
	{
		public string Text { get; set; }
		public double Confidence { get; set; }
	}

	public class DenseCaptionsResult
	{
		public List<Value> Values { get; set; }
	}
}
