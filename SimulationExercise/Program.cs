using Microsoft.Extensions.DependencyInjection;
using SimulationExercise.IOC;

ServiceCollection services = new ServiceCollection();
DependencyInjection.InjectServices(services);
DependencyInjection.InjectValidators(services);

