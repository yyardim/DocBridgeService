using DocBridgeService.Services;

public class XMLFileParserTests
{
    [Fact]
    public async Task ParseAsync_ShouldReturnParsedFileData_ForValidXmlFile()
    {
        // Arrange
        var filePath = "/mock-location/test.xml";
        var xmlContent = "<test>content</test>";
        File.WriteAllText(filePath, xmlContent);

        var parser = new XMLFileParser();

        // Act
        var result = await parser.ParseAsync(filePath);

        // Assert
        Assert.Equal("test.xml", result.FileName);
        Assert.Equal(xmlContent, result.Content);
        Assert.Equal("/mock-location", result.Location);
    }
}
