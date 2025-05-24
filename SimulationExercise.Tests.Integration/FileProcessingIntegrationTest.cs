using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Console;
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
            string inDirectoryPath = Path.Combine(Environment.CurrentDirectory, "INTestNoErrors");
            string outDirectoryPath = Path.Combine(Environment.CurrentDirectory, "OUTTestNoErrors");
            string inFilePath = Path.Combine(inDirectoryPath, "CSVTestNoErrors.csv");
            string outErrorFilePath = Path.Combine(outDirectoryPath, "Errors.log");

            if (Directory.Exists(inDirectoryPath)) Directory.Delete(inDirectoryPath, true);
            Directory.CreateDirectory(inDirectoryPath);

            if (Directory.Exists(outDirectoryPath)) Directory.Delete(outDirectoryPath, true);
            Directory.CreateDirectory(outDirectoryPath);

            using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
            {
                inputStream.Position = 0;
                inputStream.CopyTo(fileStream);
            }

            try
            {
                // Act
                _sut.ProcessFile(inDirectoryPath, outDirectoryPath);

                // Assert
                var exportDirectories = Directory.GetDirectories(outDirectoryPath);
                var resultOutFilePath = Path.Combine(exportDirectories[0], "AverageProvinceData.csv");
                var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");
                var resultOutputText = File.ReadAllText(resultOutFilePath).Trim();
                var emptyErrorFileOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

                Assert.Single(exportDirectories);
                Assert.Empty(emptyErrorFileOutputText);
                Assert.True(File.Exists(resultOutFilePath));
                Assert.Equal(expectedOutputText.Trim(), resultOutputText);
            }
            catch (Exception ex)
            {
                // Teardown
                Directory.Delete(outDirectoryPath, true);
                Directory.Delete(inDirectoryPath, true);
            }
        }

        [Theory]
        [MemberData(nameof(StreamData.InvalidStreamGenerator), MemberType = typeof(StreamData))]
        public void Program_ShouldReturnError_WhenNoConsistentReadingCreated(Stream streamInputWithErrors, string expectedErrorsText)
        {
            // Arrange
            string inErrorDirectoryPath = Path.Combine(Environment.CurrentDirectory, "INTestWithErrors");
            string outErrorDirectoryPath = Path.Combine(Environment.CurrentDirectory, "OUTTestWithErrors");
            string inErrorFilePath = Path.Combine(inErrorDirectoryPath, "CSVTestErrors.csv");

            if (Directory.Exists(inErrorDirectoryPath)) Directory.Delete(inErrorDirectoryPath, true);
            Directory.CreateDirectory(inErrorDirectoryPath);

            if (Directory.Exists(outErrorDirectoryPath)) Directory.Delete(outErrorDirectoryPath, true);
            Directory.CreateDirectory(outErrorDirectoryPath);

            using (var fileStream = new FileStream(inErrorFilePath, FileMode.Create, FileAccess.Write))
            {
                streamInputWithErrors.Position = 0;
                streamInputWithErrors.CopyTo(fileStream);
            }

            try
            {
                // Act
                _sut.ProcessFile(inErrorDirectoryPath, outErrorDirectoryPath);

                // Assert
                var exportDirectories = Directory.GetDirectories(outErrorDirectoryPath);
                var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");

                var resultErrorOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

                Assert.Single(exportDirectories);
                Assert.True(File.Exists(resultOutErrorFilePath));
                Assert.Equal(expectedErrorsText.Trim(), resultErrorOutputText);
            }
            catch (Exception ex)
            {
                // Teardown
                Directory.Delete(outErrorDirectoryPath, true);
                Directory.Delete(inErrorDirectoryPath, true);
            }
        }
    }
}