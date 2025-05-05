using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Contracts;
using SimulationExercise.IOC;

ServiceCollection services = new ServiceCollection();
DependencyInjection.InjectServices(services);
DependencyInjection.InjectValidators(services);
services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

using var serviceProvider = services.BuildServiceProvider();
var fileProcessingService = serviceProvider.GetRequiredService<IFileProcessingService>();

fileProcessingService.ProcessFile(
    @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise\IN",
    @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise\OUT");