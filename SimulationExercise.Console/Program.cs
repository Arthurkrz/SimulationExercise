using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.IOC;
using SimulationExercise.Services;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                      .WriteTo.Console()
                                      .WriteTo.Map(_ => LogPathHolder.ErrorLogPath,
                                                  (path, config) => config.File(path,
                                                   restrictedToMinimumLevel: LogEventLevel.Error,
                                                   outputTemplate: "{Level:u3}: {Message:lj}{NewLine}"))
                                      .CreateLogger();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

var connectionString = config.GetConnectionString("DefaultDatabase");
var contextFactory = new DapperContextFactory(connectionString);

RepositoryInitializer repositoryInitializer = new RepositoryInitializer();

repositoryInitializer.Initialize(contextFactory.Create());

ServiceCollection services = new ServiceCollection();
DependencyInjection.InjectServices(services);
DependencyInjection.InjectValidators(services);

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

using var serviceProvider = services.BuildServiceProvider();
var fileProcessingService = serviceProvider.GetRequiredService<IFileProcessingService>();

fileProcessingService.ProcessFile(
    Path.Combine(Path.GetTempPath(), "IN"),
    Path.Combine(Path.GetTempPath(), "OUT"));