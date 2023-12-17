using PoeThePoet.Clients.Interfaces;
using PoeThePoet.Services.Interfaces;
using PoeThePoet.Services;

namespace PoeThePoet.Tests.Services;

public class PoemGeneratorServiceTests
{
	private readonly PoemGeneratorService _poemGeneratorService;
	private readonly Mock<IOpenAIClient> _openAiClientMock;
	private readonly Mock<IImageProcessorService> _imageProcessorServiceMock;
	
	public PoemGeneratorServiceTests()
	{
		_openAiClientMock = new Mock<IOpenAIClient>();
		_imageProcessorServiceMock = new Mock<IImageProcessorService>();
		_poemGeneratorService = new PoemGeneratorService(
			_openAiClientMock.Object,
			_imageProcessorServiceMock.Object);
	}
	
	[Fact]
	public async Task GeneratePoemFromImageAsync_ShouldReturnPoem()
	{
		// Arrange
		var testImageStream = new MemoryStream();
		var testImageDescription = "Test image description";
		var testMetaPrompt = "You are a poet, weaving emotions into words.";
		var testPrompt = $"Compose an evocative and imaginative poem for the provided scenes. Capture the essence of each moment with vivid language and creative flair. {testImageDescription} For each scene, envision the emotions, details, and stories that unfold. Include the title of the poem at the beginning. Ensure that the poem follows this specific structure:\n\n[Your Title]\n\n[Your poetic lines]\n\n[More expressive lines]\n\n[Concluding lines]\n\nPlease note: Your poem must consist of exactly three stanzas.\n\nLet your creativity flow, but remember to adhere to this structure. Transport us into these captivating moments with your verses.";
	
		_imageProcessorServiceMock.Setup(x => x.GetImageDescriptionAsync(testImageStream))
			.ReturnsAsync(testImageDescription);
	
		_openAiClientMock.Setup(c => c.GetTextCompletion(testMetaPrompt, testPrompt, It.IsAny<string>(), It.IsAny<float>(), It.IsAny<int>()))
			.ReturnsAsync("Generated poem");
	
		// Act
		var result = await _poemGeneratorService.GeneratePoemFromImageAsync(testImageStream);
	
		// Assert
		Assert.Equal("Generated poem", result);

		_imageProcessorServiceMock.Verify(x => x.GetImageDescriptionAsync(testImageStream), Times.Once);

		_openAiClientMock.Verify(x => x.GetTextCompletion(
			testMetaPrompt,
			testPrompt,
			It.IsAny<string>(),
			It.IsAny<float>(),
			It.IsAny<int>()),
			Times.Once);
	}
}
