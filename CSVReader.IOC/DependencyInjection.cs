using ConsoleMenu.Contracts;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.Entities;
using CSVReader.Core.Validators;
using CSVReader.Infrastructure;
using CSVReader.Infrastructure.Repository;
using CSVReader.Services;
using CSVReader.Services.Factories;
using CSVReader.Services.Factory;
using CSVReader.Services.Handlers;
using CSVReader.Services.Utilities;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CSVReader.IOC
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