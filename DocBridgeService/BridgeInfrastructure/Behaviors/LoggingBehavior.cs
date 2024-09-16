namespace BridgeInfrastructure.Behaviors;

public class LoggingBehavior(ILogger logger)
{
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Accepts a request and a function that returns a response, 
    /// Logs the request information
    /// Runs a timer to measure the time taken to process the request
    /// Logs the time taken to process the request
    /// Returns the response.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="action"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<TResponse> LogAsync<TRequest, TResponse>
        (Func<Task<TResponse>> action, TRequest request)
    {
        _logger.Information($"Handling {typeof(TRequest).Name}");

        var stopwatch = Stopwatch.StartNew();

        var response = await action();
        
        stopwatch.Stop();

        _logger.Information($"Handled {typeof(TRequest).Name} in {stopwatch.ElapsedMilliseconds}ms");

        return response;
    }
}
