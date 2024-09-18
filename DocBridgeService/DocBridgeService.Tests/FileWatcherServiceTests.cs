using DocBridgeService.Models;
using DocBridgeService.Services;
using Moq;
using BridgeInfrastructure.Behaviors;

namespace DocBridgeService.Tests;

public class FileWatcherServiceTests
{
    private readonly Mock<IApiService> _apiServiceMock;
    private readonly Mock<LoggingBehavior> _loggingBehaviorMock;
    private readonly Mock<ValidatorBehavior<FileData>> _validatorBehaviorMock;
    private readonly IDictionary<string, Mock<IFileParser>> _fileParsersMock;

    public FileWatcherServiceTests()
    {
        _apiServiceMock = new Mock<IApiService>();
        _loggingBehaviorMock = new Mock<LoggingBehavior>();
        _validatorBehaviorMock = new Mock<ValidatorBehavior<FileData>>();

        _fileParsersMock = new Dictionary<string, Mock<IFileParser>>
        {
            { ".xml", new Mock<IFileParser>() },
            { ".csv", new Mock<IFileParser>() }
        };
    }

    [Fact]
    public async Task StartWatchingAsync_ShouldParseAndSendFileData_WhenXmlFileIsCreated()
    {
        // Arrange
        var locationsToWatch = new[] { "/mock-location" };
        var fileParsers = new Dictionary<string, IFileParser>
        {
            { ".xml", _fileParsersMock[".xml"].Object },
            { ".csv", _fileParsersMock[".csv"].Object }
        };

        var fileWatcherService = new FileWatcherService(fileParsers, _apiServiceMock.Object,
            _loggingBehaviorMock.Object, _validatorBehaviorMock.Object, locationsToWatch);

        var fileData = new FileData
        {
            FileName = "test.xml",
            Content = "<test>content</test>",
            Location = "/mock-location"
        };

        // Setup the mock to return parsed file data
        _fileParsersMock[".xml"].Setup(p => p.ParseAsync(It.IsAny<string>())).ReturnsAsync(fileData);

        // Act
        await fileWatcherService.OnFileCreated("/mock-location/test.xml");

        // Assert
        _fileParsersMock[".xml"].Verify(p => p.ParseAsync(It.IsAny<string>()), Times.Once);
        _apiServiceMock.Verify(a => a.SendToRepoCentralAsync(It.IsAny<FileData>()), Times.Once);
    }

    [Fact]
    public async Task StartWatchingAsync_ShouldParseAndSendFileData_WhenCsvFileIsCreated()
    {
        // Arrange
        var locationsToWatch = new[] { "/mock-location" };
        var fileParsers = new Dictionary<string, IFileParser>
        {
            { ".xml", _fileParsersMock[".xml"].Object },
            { ".csv", _fileParsersMock[".csv"].Object }
        };

        var fileWatcherService = new FileWatcherService(fileParsers, _apiServiceMock.Object,
            _loggingBehaviorMock.Object, _validatorBehaviorMock.Object, locationsToWatch);

        var fileData = new FileData
        {
            FileName = "test.csv",
            Content = "column1,column2\nvalue1,value2",
            Location = "/mock-location"
        };

        // Setup the mock to return parsed file data
        _fileParsersMock[".csv"].Setup(p => p.ParseAsync(It.IsAny<string>())).ReturnsAsync(fileData);

        // Act
        await fileWatcherService.OnFileCreated("/mock-location/test.csv");

        // Assert
        _fileParsersMock[".csv"].Verify(p => p.ParseAsync(It.IsAny<string>()), Times.Once);
        _apiServiceMock.Verify(a => a.SendToRepoCentralAsync(It.IsAny<FileData>()), Times.Once);
    }
}
