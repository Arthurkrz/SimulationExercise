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

namespace SimulationExercise.IOC
{
    public static class DependencyInjection
    {
        public static void InjectFactories(this IServiceCollection services)
        {
            services.AddScoped<IConsistentReadingFactory, ConsistentReadingFactory>();
            services.AddScoped<IAverageProvinceDataFactory, AverageProvinceDataFactory>();
            services.AddScoped<IContextFactory, DapperContextFactory>();
            services.AddScoped<IReadingInsertDTOFactory, ReadingInsertDTOFactory>();
            services.AddScoped<IConsistentReadingInsertDTOFactory, ConsistentReadingInsertDTOFactory>();
            services.AddScoped<IConsistentReadingExportDTOFactory, ConsistentReadingExportDTOFactory>();
        }

        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IReadingImportService, ReadingImportService>();
            services.AddScoped<IAverageProvinceDataExportService, AverageProvinceDataExportService>();
            services.AddScoped<IInputFileService, InputFileService>();
            services.AddScoped<IReadingService, ReadingService>();
            services.AddScoped<IConsistentReadingService, ConsistentReadingService>();
            services.AddScoped<IOutputFileService, OutputFileService>();
            services.AddScoped<IFilePersistanceService, FilePersistanceService>();
        }

        public static void InjectRepositories(this IServiceCollection services)
        {
            services.AddScoped<IInputFileRepository, InputFileRepository>();
            services.AddScoped<IReadingRepository, ReadingRepository>();
            services.AddScoped<IConsistentReadingRepository, ConsistentReadingRepository>();
            services.AddScoped<IOutputFileRepository, OutputFileRepository>();
        }

        public static void InjectValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Reading>, ReadingValidator>();
        }
    }
}