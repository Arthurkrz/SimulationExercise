using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimulationExercise.Infrastructure.Repository;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Validators;
using SimulationExercise.Services;
using SimulationExercise.Services.Factory;
using SimulationExercise.Infrastructure;
using SimulationExercise.Services.Factories;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Infrastructure;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Services.Utilities;
using ConsoleMenu.Contracts;
using SimulationExercise.Services.Handlers;

namespace SimulationExercise.IOC
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectFactories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddScoped<IConsistentReadingFactory, ConsistentReadingFactory>();
            services.AddScoped<IAverageProvinceDataFactory, AverageProvinceDataFactory>();
            services.AddScoped<IContextFactory, DapperContextFactory>();
            services.AddScoped<IReadingInsertDTOFactory, ReadingInsertDTOFactory>();
            services.AddScoped<IConsistentReadingInsertDTOFactory, ConsistentReadingInsertDTOFactory>();
            services.AddScoped<IConsistentReadingExportDTOFactory, ConsistentReadingExportDTOFactory>();
            services.AddScoped<IAverageProvinceDataExportDTOFactory, AverageProvinceDataExportDTOFactory>();

            return services;
        }

        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<ILogSetupService, LogSetupService>();
            services.AddScoped<IReadingImportService, ReadingImportService>();
            services.AddScoped<IAverageProvinceDataExportService, AverageProvinceDataExportService>();
            services.AddScoped<IConsistentReadingExportService, ConsistentReadingExportService>();
            services.AddScoped<IInputFileService, InputFileService>();
            services.AddScoped<IReadingService, ReadingService>();
            services.AddScoped<IConsistentReadingService, ConsistentReadingService>();
            services.AddScoped<IOutputFileService, OutputFileService>();
            services.AddScoped<IFilePersistanceService, FilePersistanceService>();
            services.AddScoped<IAverageProvinceDataService, AverageProvinceDataService>();

            return services;
        }

        public static IServiceCollection InjectHandlers(this IServiceCollection services)
        {
            services.AddScoped<IConsoleMenuHandler, InitializeHandler>();
            services.AddScoped<IConsoleMenuHandler, ExportConsistentReadingHandler>();
            services.AddScoped<IConsoleMenuHandler, ExportAverageProvinceDataHandler>();

            return services;
        }

        public static IServiceCollection InjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IInputFileRepository, InputFileRepository>();
            services.AddScoped<IReadingRepository, ReadingRepository>();
            services.AddScoped<IConsistentReadingRepository, ConsistentReadingRepository>();
            services.AddScoped<IAverageProvinceDataRepository, AverageProvinceDataRepository>();
            services.AddScoped<IOutputFileRepository, OutputFileRepository>();

            return services;
        }

        public static IServiceCollection InjectValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Reading>, ReadingValidator>();
            services.AddScoped<IValidator<ProvinceData>, ProvinceDataValidator>();

            return services;
        }
    }
}