using DocBridgeService.Services;

public class CsvFileParserTests
{
    [Fact]
    public async Task ParseAsync_ShouldReturnParsedFileData_ForValidCsvFile()
    {
        // Arrange
        var filePath = "/mock-location/test.csv";
        var csvContent = "column1,column2\nvalue1,value2";
        File.WriteAllText(filePath, csvContent);

        var parser = new CsvFileParser();

        // Act
        var result = await parser.ParseAsync(filePath);

        // Assert
        Assert.Equal("test.csv", result.FileName);
        Assert.Equal(csvContent, result.Content);
        Assert.Equal("/mock-location", result.Location);
    }
}
