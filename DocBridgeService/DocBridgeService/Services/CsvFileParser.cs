namespace DocBridgeService.Services;

public class CsvFileParser : IFileParser
{
    public async Task<FileData> ParseAsync(string filePath)
    {
        var csvContent = await File.ReadAllTextAsync(filePath);

        return new FileData
        {
            FileName = Path.GetFileName(filePath),
            Content = csvContent,
            Location = Path.GetDirectoryName(filePath)
        };
    }
}
