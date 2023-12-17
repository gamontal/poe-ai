namespace SnapPoet.Services.Interfaces;

public interface IImageProcessorService
{
    Task<string> GetImageDescriptionAsync(MemoryStream imageStream);
}
