namespace PoeThePoet.Services.Interfaces;

public interface IImageProcessorService
{
	Task<string> GetImageDescriptionAsync(MemoryStream imageStream);
}
