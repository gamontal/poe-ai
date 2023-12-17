using PoeThePoet.Clients;
using PoeThePoet.Services.Interfaces;

namespace PoeThePoet.Services;

public class PoemGeneratorService : IPoemGeneratorService
{
	private readonly OpenAIClient _openAiClient;
	private readonly IImageProcessorService _imageProcessorService;

	public PoemGeneratorService(
		OpenAIClient openAiClient,
		IImageProcessorService imageProcessorService)
	{
		_openAiClient = openAiClient;
		_imageProcessorService = imageProcessorService;
	}

	public async Task<string?> GeneratePoemFromImageAsync(MemoryStream imageStream)
	{
		var imageDescription = await _imageProcessorService.GetImageDescriptionAsync(imageStream);
		var poem = await _openAiClient.GetTextCompletion("You are a poet, weaving emotions into words.", $"Compose an evocative and imaginative poem for the provided scenes. Capture the essence of each moment with vivid language and creative flair. {imageDescription} For each scene, envision the emotions, details, and stories that unfold. Include the title of the poem at the beginning. Ensure that the poem follows this specific structure:\n\n[Your Title]\n\n[Your poetic lines]\n\n[More expressive lines]\n\n[Concluding lines]\n\nPlease note: Your poem must consist of exactly three stanzas.\n\nLet your creativity flow, but remember to adhere to this structure. Transport us into these captivating moments with your verses.");

		return poem;
	}
}
