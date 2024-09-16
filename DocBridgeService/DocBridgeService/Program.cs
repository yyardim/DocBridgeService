//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();


using Serilog;
using Amazon.CloudWatchLogs;
using BridgeInfrastructure.Behaviors;
using FluentValidation;
using Serilog.Sinks.AwsCloudWatch;

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
builder.Services.AddTransient(typeof(ValidatorBehavior<,>));

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register other services

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();
