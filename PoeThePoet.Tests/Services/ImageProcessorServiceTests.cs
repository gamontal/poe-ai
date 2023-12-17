using Bogus;
using PoeThePoet.Clients.Interfaces;
using PoeThePoet.Services;
using static PoeThePoet.Clients.ComputerVisionClient;

namespace PoeThePoet.Tests.Services;

public class ImageProcessorServiceTests
{
	private readonly Mock<IComputerVisionClient> _computerVisionClientMock;
	private readonly ImageProcessorService _imageProcessorService;

	public ImageProcessorServiceTests()
	{
		_computerVisionClientMock = new Mock<IComputerVisionClient>();
		_imageProcessorService = new ImageProcessorService(_computerVisionClientMock.Object);
	}

	[Fact]
	public async Task GetImageDescriptionAsync_ShouldReturnImageDescription()
	{
		// Arrange
		using var testImageStream = new MemoryStream();
		var testDenseCaptions = GenerateDenseCaptions();

		_computerVisionClientMock.Setup(c => c.GetImageDenseCaptions(It.IsAny<MemoryStream>()))
			.ReturnsAsync(testDenseCaptions);

		// Act
		var result = await _imageProcessorService.GetImageDescriptionAsync(testImageStream);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<string>(result);
		Assert.NotEmpty(result);
	}

	private static DenseCaptions GenerateDenseCaptions()
	{
		var testValues = new Faker<Value>()
			.RuleFor(o => o.Text, f => f.Lorem.Sentence())
			.RuleFor(o => o.Confidence, f => f.Random.Double(0, 1));

		var testDenseCaptionsResult = new Faker<DenseCaptionsResult>()
			.RuleFor(o => o.Values, f => testValues.Generate(3));

		return new Faker<DenseCaptions>()
			.RuleFor(o => o.DenseCaptionsResult, f => testDenseCaptionsResult.Generate())
			.RuleFor(o => o.ModelVersion, f => f.Lorem.Text())
			.Generate();
	}
}