using BridgeInfrastructure.Behaviors;
using DocBridgeService.Models;

namespace DocBridgeService.Services;

public class FileWatcherService(IDictionary<string, IFileParser> fileParsers, IApiService apiService,
    LoggingBehavior loggingBehavior, ValidatorBehavior<FileData> validatorBehavior, string[] locationsToWatch) : IFileWatcherService
{
    private readonly IDictionary<string, IFileParser> _fileParsers = fileParsers;
    private readonly IApiService _apiService = apiService;
    private readonly LoggingBehavior _loggingBehavior = loggingBehavior;
    private readonly ValidatorBehavior<FileData> _validatorBehavior = validatorBehavior;
    private readonly string[] _locationsToWatch = locationsToWatch;

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

    public async Task OnFileCreated(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        if (_fileParsers.TryGetValue(extension, out var parser))
        {
            await _loggingBehavior.LogAsync(async () =>
            {
                var fileData = await parser.ParseAsync(filePath);

                // Validation step
                await _validatorBehavior.ValidateAsync(fileData);

                // Send to RepoCentral
                return await _apiService.SendToRepoCentralAsync(fileData);

            }, filePath);
        }
        else
        {
            Console.WriteLine($"No parser available for files with extension {extension}");
        }
    }
}
