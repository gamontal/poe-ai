using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace SnapPoet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageAnalysisController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _cvHttpClient;
        private readonly HttpClient _openAiHttpClient;
        private readonly ILogger<ImageAnalysisController> _logger;

        public ImageAnalysisController(
            IConfiguration configuration,
            ILogger<ImageAnalysisController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _cvHttpClient = httpClientFactory.CreateClient("ComputerVisionClient");
            _cvHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration["AzureCognitiveServices:ComputerVision:Key"]);
            _openAiHttpClient = httpClientFactory.CreateClient("OpenAiClient");
            _openAiHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["OpenAi:Key"]}");
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using var stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            var byteContent = new ByteArrayContent(stream.ToArray());
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var cvResponse = await _cvHttpClient.PostAsync(
                "computervision/imageanalysis:analyze?api-version=2023-02-01-preview&features=denseCaptions",
                byteContent);

            if (cvResponse.IsSuccessStatusCode)
            {
                var cvResponseContent = await cvResponse.Content.ReadAsStringAsync();
                var cvDenseCaptionResult = JsonConvert.DeserializeObject<IAResponse>(cvResponseContent);

                StringBuilder openaiPromptBuilder = new StringBuilder();

                foreach (var value in cvDenseCaptionResult?.DenseCaptionsResult.Values)
                {
                    openaiPromptBuilder.AppendLine($"{value.Text}");
                }

                string cvTextContent = openaiPromptBuilder.ToString().Trim();

                string openAiPrompt = $"I have a program that analyzes images and describes them in detail. The image description is:\n\n{cvTextContent}\n\nI want to generate a poem based on the image description I provide. Generate a poem for the provided image description. Do not respond with anything else other than the poem. The poem must start with its title. The title needs to be short. The poem needs to have 3 stanzas, no more no less. The poem is:";
                
                var openAiCompletionRequest = new StringContent(
                    JsonConvert.SerializeObject(new OpenAiCompletionRequest
                    {
                        Model = "text-davinci-003",
                        Prompt = openAiPrompt,
                        Temperature = 1,
                        MaxTokens = 150
                    }),
                    Encoding.UTF8,
                    "application/json");

                var openAiResponse = await _openAiHttpClient.PostAsync(
                    "completions", openAiCompletionRequest);

                var openAiResponseContent = await openAiResponse.Content.ReadAsStringAsync(); 
                
                var openAiCompletionResult = JsonConvert.DeserializeObject<OpenAiCompletionResponse>(openAiResponseContent);

                if (openAiCompletionResult?.Choices.FirstOrDefault() is null)
                {
                    return Ok("My apologies, I could not come up with anything...");
                }

                return Ok(openAiCompletionResult.Choices.First().Text);
            }

            return Problem();
        }

        public class Value
        {
            public string? Text { get; set; }
            public double Confidence { get; set; }
        }

        public class DenseCaptionsResult
        {
            public List<Value> Values { get; set; }
        }

        public class IAResponse
        {
            public DenseCaptionsResult DenseCaptionsResult { get; set; }
            public string ModelVersion { get; set; }
        }

        public class OpenAiCompletionRequest
        {
            [JsonProperty("model")]
            public string Model { get; set; }

            [JsonProperty("prompt")]
            public string Prompt { get; set; }

            [JsonProperty("temperature")]
            public int Temperature { get; set; }

            [JsonProperty("max_tokens")]
            public int MaxTokens { get; set;}
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
}