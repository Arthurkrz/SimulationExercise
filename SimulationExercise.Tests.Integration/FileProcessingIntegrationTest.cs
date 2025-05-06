using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts;
using SimulationExercise.IOC;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Integration
{
    public class FileProcessingIntegrationTest
    {
        private readonly IFileProcessingService _sut;
        private readonly ServiceProvider _serviceProvider;

        public FileProcessingIntegrationTest()
        {
            var serviceCollection = new ServiceCollection();

            DependencyInjection.InjectServices(serviceCollection);
            DependencyInjection.InjectValidators(serviceCollection);

            serviceCollection.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Information);
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<IFileProcessingService>();
        }

        [Fact]
        public void Program_ShouldExportFiles_WithNoErrors()
        {
            // Arrange
            string outTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTest";

            // Act
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTest",
                             outTestDirectoryPath);

            var outFiles = Directory.GetFiles(outTestDirectoryPath);

            // Assert
            Assert.Single(outFiles);

            // Teardown
            Directory.Delete(outTestDirectoryPath, true);
        }

        [Fact]
        public void Program_ShouldReturnError_WhenNoAverageProvinceDataCreated()
        {
            // Arrange
            string outTestErrorDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTestErrors";

            // Act
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTestErrors", 
                outTestErrorDirectoryPath);

            var outFiles = Directory.GetFiles(outTestErrorDirectoryPath);

            // Assert
            Assert.True(outFiles.Length == 2);

            // Teardown
            Directory.Delete(outTestErrorDirectoryPath, true);
        }
    }
}