using Microsoft.AspNetCore.Mvc;
using PoeThePoet.Services.Interfaces;

namespace PoeThePoet.Controllers;

[ApiController]
[Route("[controller]")]
public class PoemsController : ControllerBase
{
	private readonly ILogger<PoemsController> _logger;
	private readonly IPoemGeneratorService _poemGeneratorService;

	public PoemsController(
		ILogger<PoemsController> logger,
		IPoemGeneratorService poemGeneratorService)
	{
		_logger = logger;
		_poemGeneratorService = poemGeneratorService;
	}

	[HttpPost]
	[Route("generate")]
	public async Task<IActionResult> Post()
	{
		try
		{
			using var stream = new MemoryStream();
			await Request.Body.CopyToAsync(stream);

			var poem = await _poemGeneratorService.GeneratePoemFromImageAsync(stream);
			return Ok(poem);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error generating poem from image");
			return Ok("My apologies, I could not come up with anything...");
		}
	}
}
