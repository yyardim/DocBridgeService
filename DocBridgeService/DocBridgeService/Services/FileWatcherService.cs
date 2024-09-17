namespace DocBridgeService.Services;

public class FileWatcherService : IFileWatcherService
{
    private readonly IFileParser _fileParser;
    private readonly IApiService _apiService;
    private readonly string[] _locationsToWatch;

    public FileWatcherService(IFileParser fileParser, IApiService apiService, string[] locationsToWatch)
    {
        _fileParser = fileParser;
        _apiService = apiService;
        _locationsToWatch = locationsToWatch;
    }

    public async Task StartWatchingAsync()
    {
        foreach (var location in _locationsToWatch)
        {
            var watcher = new FileSystemWatcher(location)
            {
                Filter = "*.*",
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            watcher.Created += async (sender, e) =>
            {
                await OnFileCreated(e.FullPath);
            };
        }
    }

    // Simulate file created event for testing
    public async Task OnFileCreated(string filePath)
    {
        var fileData = await _fileParser.ParseAsync(filePath);
        await _apiService.SendToRepoCentralAsync(fileData);
    }
}
