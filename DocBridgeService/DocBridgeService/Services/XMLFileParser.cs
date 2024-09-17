using DocBridgeService.Models;
using System.Xml.Linq;

namespace DocBridgeService.Services;

public class XMLFileParser : IFileParser
{
    public async Task<FileData> ParseAsync(string filePath)
    {
        var xmlContent = await File.ReadAllTextAsync(filePath);
        var xmlDoc = XDocument.Parse(xmlContent);

        return new FileData
        {
            FileName = Path.GetFileName(filePath),
            Content = xmlDoc.ToString(),
            Location = Path.GetDirectoryName(filePath)
        };
    }
}
