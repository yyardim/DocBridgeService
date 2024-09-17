using DocBridgeService.Models;

namespace DocBridgeService.Services;

public interface IFileParser
{
    Task<FileData> ParseAsync(string filePath);
}
