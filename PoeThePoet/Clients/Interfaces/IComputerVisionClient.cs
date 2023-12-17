using static PoeThePoet.Clients.ComputerVisionClient;

namespace PoeThePoet.Clients.Interfaces;

public interface IComputerVisionClient
{
	Task<DenseCaptions?> GetImageDenseCaptions(MemoryStream imageStream);
}
