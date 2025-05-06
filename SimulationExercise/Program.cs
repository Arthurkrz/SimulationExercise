using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Contracts;
using SimulationExercise.IOC;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                      .WriteTo.Console()
                                      .WriteTo.File("PATH", restrictedToMinimumLevel: LogEventLevel.Error)
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

Log.CloseAndFlush();