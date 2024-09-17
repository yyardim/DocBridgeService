using DocBridgeService.Models;
using DocBridgeService.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocBridgeService.Tests;

public class FileWatcherServiceTests
{
    private readonly Mock<IFileParser> _fileParserMock;
    private readonly Mock<IApiService> _apiServiceMock;

    public FileWatcherServiceTests()
    {
        _fileParserMock = new Mock<IFileParser>();
        _apiServiceMock = new Mock<IApiService>();
    }

    [Fact]
    public async Task StartWatchingAsync_ShouldParseAndSendFileData_WhenFileIsCreated()
    {
        // Arrange
        var locationsToWatch = new[] { "/mock-location" };
        var fileWatcherService = new FileWatcherService(_fileParserMock.Object, _apiServiceMock.Object, locationsToWatch);

        var fileData = new FileData
        {
            FileName = "test.xml",
            Content = "<test>content</test>",
            Location = "/mock-location"
        };

        // Setup the mock to return parsed file data
        _fileParserMock.Setup(p => p.ParseAsync(It.IsAny<string>())).ReturnsAsync(fileData);

        // Act
        // Simulate that a file is being created by calling the internal method manually.
        await fileWatcherService.OnFileCreated("/mock-location/test.xml");

        // Assert
        _fileParserMock.Verify(p => p.ParseAsync(It.IsAny<string>()), Times.Once);
        _apiServiceMock.Verify(a => a.SendToRepoCentralAsync(It.IsAny<FileData>()), Times.Once);
    }
}
