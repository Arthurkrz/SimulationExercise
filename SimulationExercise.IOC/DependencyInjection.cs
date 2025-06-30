using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimulationExercise.Architecture.Repository;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Validators;
using SimulationExercise.Services;
using SimulationExercise.Services.Factory;

namespace SimulationExercise.IOC
{
    public static class DependencyInjection
    {
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IConsistentReadingFactory, ConsistentReadingFactory>();
            services.AddScoped<IAverageProvinceDataExportService, AverageProvinceDataExportService>();
            services.AddScoped<IAverageProvinceDataFactory, AverageProvinceDataFactory>();
            services.AddScoped<IProvinceDataListFactory, ProvinceDataListFactory>();
            services.AddScoped<IReadingImportService, ReadingImportService>();
            services.AddScoped<IFileProcessingService, FileProcessingService>();
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