using Serilog;
using Amazon.CloudWatchLogs;
using BridgeInfrastructure.Behaviors;
using Serilog.Sinks.AwsCloudWatch;
using DocBridgeService.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog and CloudWatch setup
var options = new CloudWatchSinkOptions
{
    LogGroupName = "Put Logic Here",
    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
    CreateLogGroup = true,
    LogStreamNameProvider = new DefaultLogStreamProvider(),
    TextFormatter = new Serilog.Formatting.Json.JsonFormatter()
};

var cloudWatchClient = new AmazonCloudWatchLogsClient(
    new Amazon.Runtime.BasicAWSCredentials("your-access-key", "your-secret-key"),
    Amazon.RegionEndpoint.USEast1
);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.AmazonCloudWatch(options, cloudWatchClient)
        .WriteTo.Console();
});

// Register LoggingBehavior and ValidatorBehavior globally
builder.Services.AddSingleton<LoggingBehavior>();
builder.Services.AddTransient(typeof(ValidatorBehavior<>));
builder.Services.AddTransient<IFileWatcherService, FileWatcherService>();
builder.Services.AddTransient<IApiService, ApiService>();

// Register multiple parsers
builder.Services.AddTransient<XMLFileParser>();
builder.Services.AddTransient<CsvFileParser>();
builder.Services.AddSingleton<IDictionary<string, IFileParser>>(serviceProvider =>
    new Dictionary<string, IFileParser>
    {
        { ".xml", serviceProvider.GetRequiredService<XMLFileParser>() },
        { ".csv", serviceProvider.GetRequiredService<CsvFileParser>() }
    });

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri("https://repocentral/api");
});

var app = builder.Build();

// Start FileWatcherService
var fileWatcherService = app.Services.GetRequiredService<IFileWatcherService>();
Task.Run(() => fileWatcherService.StartWatchingAsync());

app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();