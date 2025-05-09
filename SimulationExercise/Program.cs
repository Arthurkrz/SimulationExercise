using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Map;
using SimulationExercise.Core.Contracts;
using SimulationExercise.IOC;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                      .WriteTo.Console()
                                      .WriteTo.Map(_ => SimulationExercise.Console.LogPathHolder.ErrorLogPath, 
                                                  (path, config) => config.File(path, 
                                                   restrictedToMinimumLevel: LogEventLevel.Error))
                                      .CreateLogger();

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
    @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise\IN",
    @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise\OUT");