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
        private readonly Mock<ILogger<FileProcessingService>> _loggerMock;

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
            _loggerMock = new Mock<ILogger<FileProcessingService>>();
        }

        [Fact]
        public void FileProcessingService_ShouldExportFiles_WithNoErrors()
        {
            // Arrange
            string outTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTest";

            // Act
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTest",
                             outTestDirectoryPath);

            var outFiles = Directory.GetFiles(outTestDirectoryPath);

            // Assert
            Assert.True(outFiles.Length > 0);
            // ASSERT DE AUSÊNCIA DE ERROS

            // Teardown
            Directory.Delete(outTestDirectoryPath);
        }

        public void FileProcessingService_ShouldReturnError_WhenNoAverageProvinceDataCreated()
        {
            // Arrange
      
        }
    }
}