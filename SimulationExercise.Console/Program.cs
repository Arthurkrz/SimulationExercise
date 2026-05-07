using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Console;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Infrastructure;
using SimulationExercise.IOC;
using SimulationExercise.Services.Utilities;
using ConsoleMenu.IOC;
using ConsoleMenu.Contracts;

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

var contextFactory = new DapperContextFactory(config);
new RepositoryInitializer().Initialize(contextFactory.Create());

var services = new ServiceCollection();

services.InjectServices()
        .InjectHandlers()
        .InjectRepositories()
        .InjectFactories(config)
        .InjectValidators()
        .AddConsoleMenu()
        .AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

using var serviceProvider = services.BuildServiceProvider();
var filePersistanceService = serviceProvider.GetRequiredService<IFilePersistanceService>();
var menuSelector = serviceProvider.GetRequiredService<IConsoleMenuSelector>();
var menuExecutor = serviceProvider.GetRequiredService<IConsoleMenuExecutor>();
var logPathSetup = serviceProvider.GetRequiredService<ILogSetupService>();

logPathSetup.Configure();

var menu = new MenuSetup(filePersistanceService, menuSelector, menuExecutor);
await menu.RunAsync();