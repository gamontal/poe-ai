using SnapPoet.Clients;
using SnapPoet.Services.Interfaces;

namespace SnapPoet.Services;

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

		var prompt = $"Compose evocative and imaginative poems for the provided scenes. Capture the essence of each moment with vivid language and creative flair. Keep in mind that each poem should consist of up to three stanzas. {imageDescription} For each scene, envision the emotions, details, and stories that unfold. Let your creativity flow within the confines of three stanzas, and transport us into these captivating moments.";
		var poem = await _openAiClient.GetCompletion(prompt);

		return poem;
	}
}
