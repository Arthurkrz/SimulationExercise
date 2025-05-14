using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.IOC;
using SimulationExercise.Tests.Integration.ObjectGenerators;

namespace SimulationExercise.Tests.Integration
{
    public class FileProcessingIntegrationTest
    {
        private readonly IFileProcessingService _sut;
        private readonly ServiceProvider _serviceProvider;

        public FileProcessingIntegrationTest()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                  .WriteTo.Console()
                                                  .WriteTo.Map(_ => LogPathHolder.ErrorLogPath,
                                                              (path, config) => config.File(path,
                                                               restrictedToMinimumLevel: LogEventLevel.Error,
                                                               outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}"))
                                                  .CreateLogger();

            ServiceCollection services = new ServiceCollection();
            DependencyInjection.InjectServices(services);
            DependencyInjection.InjectValidators(services);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            _serviceProvider = services.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<IFileProcessingService>();
        }

        [Theory]
        [MemberData(nameof(StreamData.ValidStreamGenerator), MemberType = typeof(StreamData))]
        public void Program_ShouldExportFiles_WithNoErrors(Stream inputStream, string expectedOutputText)
        {
            // Arrange
            string inTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTest";
            string outTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTest";
            string inFilePath = Path.Combine(inTestDirectoryPath, "NoErrorFile.csv");

            if (!Directory.Exists(inTestDirectoryPath)) Directory.CreateDirectory(inTestDirectoryPath);
            if (!Directory.Exists(outTestDirectoryPath)) Directory.CreateDirectory(outTestDirectoryPath);

            using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
            {
                inputStream.Position = 0;
                inputStream.CopyTo(fileStream);
            }

            // Act
            _sut.ProcessFile(inTestDirectoryPath, outTestDirectoryPath);

            var exportDirectories = Directory.GetDirectories(outTestDirectoryPath);
            var resultOutFilePath = Path.Combine(exportDirectories[0], "AverageProvinceData.csv");

            var resultOutputText = File.ReadAllText(resultOutFilePath).Trim();

            // Assert
            Assert.Single(exportDirectories);
            Assert.True(File.Exists(resultOutFilePath));
            Assert.Equal(expectedOutputText.Trim(), resultOutputText);

            // Teardown
            Directory.Delete(outTestDirectoryPath, true);
            Directory.Delete(inTestDirectoryPath, true);
        }

        [Theory]
        [MemberData(nameof(StreamData.InvalidStreamGenerator), MemberType = typeof(StreamData))]
        public void Program_ShouldReturnError_WhenNoConsistentReadingCreated(Stream streamInputWithErrors, string expectedErrorsText)
        {
            // Arrange
            string inTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTestErrors";
            string outTestErrorDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTestErrors";
            var inErrorFilePath = Path.Combine(inTestDirectoryPath, "CSVTestErrors.csv");

            if (!Directory.Exists(inTestDirectoryPath)) Directory.CreateDirectory(inTestDirectoryPath);
            if (!Directory.Exists(outTestErrorDirectoryPath)) Directory.CreateDirectory(outTestErrorDirectoryPath);

            using (var fileStream = new FileStream(inErrorFilePath, FileMode.Create, FileAccess.Write))
            {
                streamInputWithErrors.Position = 0;
                streamInputWithErrors.CopyTo(fileStream);
            }

            // Act
            _sut.ProcessFile(inTestDirectoryPath, outTestErrorDirectoryPath);

            var exportDirectories = Directory.GetDirectories(outTestErrorDirectoryPath);
            var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");

            var resultErrorOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

            // Assert
            Assert.Single(exportDirectories);
            Assert.True(File.Exists(resultOutErrorFilePath));
            Assert.Equal(expectedErrorsText.Trim(), resultErrorOutputText);

            // Teardown
            Directory.Delete(outTestErrorDirectoryPath, true);
            Directory.Delete(inTestDirectoryPath, true);
        }
    }
}