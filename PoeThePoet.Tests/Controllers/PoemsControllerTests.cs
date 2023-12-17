using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PoeThePoet.Controllers;
using PoeThePoet.Services.Interfaces;

namespace PoeThePoet.Tests.Controllers;

public class PoemsControllerTests
{
	private readonly Mock<ILogger<PoemsController>> _loggerMock;
	private readonly Mock<IPoemGeneratorService> _poemGeneratorServiceMock;

	private readonly PoemsController _poemsController;

	public PoemsControllerTests()
	{
		_loggerMock = new Mock<ILogger<PoemsController>>();
		_poemGeneratorServiceMock = new Mock<IPoemGeneratorService>();

		_poemsController = new PoemsController(
			_loggerMock.Object,
			_poemGeneratorServiceMock.Object);
	}

	[Fact]
	public async Task GeneratePoemFromImageAsync_ReturnsOk()
	{
		// Arrange
		// Act
		var result = await _poemsController.Post();

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = (OkObjectResult)result;
		Assert.Equal(200, okResult.StatusCode);
	}

	[Fact]
	public async Task GeneratePoemFromImageAsync_LogsError()
	{
		// Arrange
		_poemGeneratorServiceMock.Setup(s => s.GeneratePoemFromImageAsync(It.IsAny<MemoryStream>()))
			.Throws<Exception>();

		// Act
		var result = await _poemsController.Post();

		// Assert
		_loggerMock.Verify(
			m => m.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((o, t) => string.Equals("Error generating poem from image", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
	}

	[Fact]
	public async Task GeneratePoemFromImageAsync_ReturnsOkWithGenericMessage()
	{
		// Arrange
		_poemGeneratorServiceMock.Setup(s => s.GeneratePoemFromImageAsync(It.IsAny<MemoryStream>()))
			.Throws<Exception>();

		// Act
		var result = await _poemsController.Post();

		// Assert
		Assert.IsType<OkObjectResult>(result);
		var okResult = (OkObjectResult)result;
		Assert.Equal(200, okResult.StatusCode);
		Assert.Equal("My apologies, I could not come up with anything...", okResult.Value);
	}
}
