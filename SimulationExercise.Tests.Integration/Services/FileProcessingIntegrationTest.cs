using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Console;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.IOC;
using SimulationExercise.Tests.Integration.ObjectGenerators;

namespace SimulationExercise.Tests.Integration.Services
{
    public class FileProcessingIntegrationTest
    {
        private readonly IFileProcessingService _sut;
        private readonly ServiceProvider _serviceProvider;
        private readonly string _basePath;
        private readonly string _inDirectoryPath;
        private readonly string _outDirectoryPath;

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
            services.InjectServices();
            services.InjectValidators();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            _serviceProvider = services.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<IFileProcessingService>();

            _basePath = Path.Combine(Path.GetTempPath(), "SimulationExerciseTests");
            _inDirectoryPath = Path.Combine(_basePath, "INTest");
            _outDirectoryPath = Path.Combine(_basePath, "OUTTest");
        }

        [Theory]
        [MemberData(nameof(StreamData.ValidStreamGenerator), MemberType = typeof(StreamData))]
        public void Program_ShouldExportFiles_WithNoErrors(Stream inputStream, string expectedOutputText)
        {
            // Arrange
            string inFilePath = Path.Combine(_inDirectoryPath, "CSVTest.csv");
            DirectoryCleanup();

            using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
            {
                inputStream.Position = 0;
                inputStream.CopyTo(fileStream);
            }

            // Act
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

            // Assert
            var exportDirectories = Directory.GetDirectories(_outDirectoryPath);
            var resultOutFilePath = Path.Combine(exportDirectories[0], "AverageProvinceData.csv");
            var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");
            var resultOutputText = File.ReadAllText(resultOutFilePath).Trim();
            var emptyErrorFileOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

            Assert.Single(exportDirectories);
            Assert.Empty(emptyErrorFileOutputText);
            Assert.True(File.Exists(resultOutFilePath));
            Assert.Equal(expectedOutputText.Trim(), resultOutputText);
        }

        [Theory]
        [MemberData(nameof(StreamData.InvalidStreamGenerator), MemberType = typeof(StreamData))]
        public void Program_ShouldReturnError_WhenNoConsistentReadingCreated(Stream streamInputWithErrors, string expectedErrorsText)
        {
            // Arrange
            string inErrorFilePath = Path.Combine(_inDirectoryPath, "CSVTestErrors.csv");
            DirectoryCleanup();

            using (var fileStream = new FileStream(inErrorFilePath, FileMode.Create, FileAccess.Write))
            {
                streamInputWithErrors.Position = 0;
                streamInputWithErrors.CopyTo(fileStream);
            }

            // Act
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

            // Assert
            var exportDirectories = Directory.GetDirectories(_outDirectoryPath);
            var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");

            var resultErrorOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

            Assert.Single(exportDirectories);
            Assert.True(File.Exists(resultOutErrorFilePath));
            Assert.Equal(expectedErrorsText.Trim(), resultErrorOutputText);
        }

        private void DirectoryCleanup()
        {
            if (Directory.Exists(_inDirectoryPath)) Directory.Delete(_inDirectoryPath, true);
            Directory.CreateDirectory(_inDirectoryPath);

            if (Directory.Exists(_outDirectoryPath)) Directory.Delete(_outDirectoryPath, true);
            Directory.CreateDirectory(_outDirectoryPath);
        }
    }
}