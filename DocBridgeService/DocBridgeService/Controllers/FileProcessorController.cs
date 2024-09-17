using DocBridgeService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocBridgeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileProcessorController : ControllerBase
{
    private readonly IFileWatcherService _fileWatcherService;

    public FileProcessorController(IFileWatcherService fileWatcherService)
    {
        _fileWatcherService = fileWatcherService;
    }

    [HttpPost("start-watching")]
    public async Task<IActionResult> StartWatching()
    {
        await _fileWatcherService.StartWatchingAsync();
        return Ok("File watcher started.");
    }
}
