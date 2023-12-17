namespace PoeThePoet.Services.Interfaces;

public interface IPoemGeneratorService
{
	Task<string?> GeneratePoemFromImageAsync(MemoryStream imageStream);
}
