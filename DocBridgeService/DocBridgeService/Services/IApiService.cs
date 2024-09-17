using DocBridgeService.Models;

namespace DocBridgeService.Services;

public interface IApiService
{
    Task<RepoCentralResponse> SendToRepoCentralAsync(FileData fileData);
}
