using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimulationExercise.Core.Contracts;
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
        }

        public static void InjectValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Reading>, ReadingValidator>();
        }
    }
}
