using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using SimulationExercise.IOC;
using SimulationExercise.Services.Utilities;
using SimulationExercise.Tests.Integration.ObjectGenerators;
using SimulationExercise.Tests.Integration.Utilities;
using System.Text;

namespace SimulationExercise.Tests.Integration.Service
{
    public class FilePersistanceIntegrationTest
    {
        private readonly IFilePersistanceService _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private readonly IAverageProvinceDataRepository _averageProvinceDataRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IContextFactory _contextFactory;

        private readonly TestRepositoryCleanup _integrationTestRepositoryCleanup;
        private readonly INFileCreator _integrationTestINFileCreator;

        private readonly string _basePath;
        private readonly string _inDirectoryPath;
        private readonly string _outDirectoryPath;

        public FilePersistanceIntegrationTest()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                  .WriteTo.Console()
                                                  .WriteTo.Map(_ => LogPathHolder.ErrorLogPath,
                                                              (path, config) => config.File(path,
                                                               restrictedToMinimumLevel: LogEventLevel.Error,
                                                               outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}"))
                                                  .CreateLogger();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var connectionString = config.GetConnectionString("Default") ?? 
                throw new ArgumentNullException("Null Connection String");

            ServiceCollection services = new ServiceCollection();
            services.InjectFactories(config);
            services.InjectServices();
            services.InjectRepositories();
            services.InjectValidators();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            services.AddSingleton(connectionString);
            _serviceProvider = services.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<IFilePersistanceService>();
            _contextFactory = _serviceProvider.GetRequiredService<IContextFactory>();
            _inputFileRepository = _serviceProvider.GetRequiredService<IInputFileRepository>();
            _readingRepository = _serviceProvider.GetRequiredService<IReadingRepository>();
            _consistentReadingRepository = _serviceProvider.GetRequiredService<IConsistentReadingRepository>();
            _averageProvinceDataRepository = _serviceProvider.GetRequiredService<IAverageProvinceDataRepository>();
            _outputFileRepository = _serviceProvider.GetRequiredService<IOutputFileRepository>();

            _basePath = Path.Combine(Path.GetTempPath(), "SimulationExerciseTests.Integration");
            _inDirectoryPath = Path.Combine(_basePath, "INTest");
            _outDirectoryPath = Path.Combine(_basePath, "OUTTest");

            _integrationTestRepositoryCleanup = new TestRepositoryCleanup();
            _integrationTestINFileCreator = new INFileCreator();
        }

        [Theory]
        [MemberData(nameof(StreamData.ValidStreamGenerator), MemberType = typeof(StreamData))]
        public void AllPersistanceSteps_ShouldProcessExportAndPersistInDatabase(Stream inputStream,
                                                                                List<string> expectedInputFileLines,
                                                                                List<ReadingGetDTO> expectedReadings,
                                                                                List<ConsistentReadingGetDTO> expectedCRs,
                                                                                List<AverageProvinceDataGetDTO> expectedAPDs,
                                                                                List<string> expectedOutputFileLines)
        {
            // Arrange
            _sut.LoggerConfiguration(_outDirectoryPath);
            _integrationTestRepositoryCleanup.Cleanup();
            DirectoryCleanup();
            _integrationTestINFileCreator.CreateINFiles(_inDirectoryPath, 1, inputStream);

            // Act
            _sut.Initialize(_inDirectoryPath);
            _sut.CreateReadings();
            _sut.CreateConsistentReadings();
            _sut.CreateAverageProvinceDatas();
            _sut.CreateAverageProvinceDataOutputFiles();
            _sut.CreateConsistentReadingOutputFiles();
            _sut.ExportAverageProvinceData(_outDirectoryPath);
            _sut.ExportConsistentReadings(_outDirectoryPath);

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var inputFiles = _inputFileRepository.GetByStatusAsync(Status.Success, context);
                var readings = _readingRepository.GetByStatusAsync(Status.Success, context);
                var consistentReadings = _consistentReadingRepository.GetByIsExportedAsync(true, context);
                var averageProvinceDatas = _averageProvinceDataRepository.GetByIsExportedAsync(true, context);
                var outputFiles = _outputFileRepository.GetByIsExportedAsync(true, context);

                Assert.Single(inputFiles);
                Assert.Equal(10, readings.Count);
                Assert.Equal(10, consistentReadings.Count);
                Assert.Equal(5, averageProvinceDatas.Count);
                Assert.Single(outputFiles);

                foreach (var expected in expectedReadings)
                {
                    var reading = readings.FirstOrDefault(x => x.ReadingId == expected.ReadingId);
                    Assert.NotNull(reading);

                    Assert.Equal(expected.ReadingId, reading.ReadingId);
                    Assert.Equal(expected.InputFileId, reading.InputFileId);
                    Assert.Equal(expected.SensorId, reading.SensorId);
                    Assert.Equal(expected.SensorTypeName, reading.SensorTypeName);
                    Assert.Equal(expected.Unit, reading.Unit);
                    Assert.Equal(expected.StationId, reading.StationId);
                    Assert.Equal(expected.StationName, reading.StationName);
                    Assert.Equal(expected.Value, reading.Value);
                    Assert.Equal(expected.Province, reading.Province);
                    Assert.Equal(expected.City, reading.City);
                    Assert.Equal(expected.IsHistoric, reading.IsHistoric);
                    Assert.Equal(expected.StartDate, reading.StartDate);
                    Assert.Equal(expected.StopDate, reading.StopDate);
                    Assert.Equal(expected.UtmNord, reading.UtmNord);
                    Assert.Equal(expected.UtmEst, reading.UtmEst);
                    Assert.Equal(expected.Latitude, reading.Latitude);
                    Assert.Equal(expected.Longitude, reading.Longitude);
                    Assert.Equal(expected.Status, reading.Status);
                }

                foreach (var expectedCR in expectedCRs)
                {
                    var cr = consistentReadings.FirstOrDefault(x => x.ReadingId == expectedCR.ReadingId);
                    Assert.NotNull(cr);

                    Assert.Equal(expectedCR.SensorId, cr.SensorId);
                    Assert.Equal(expectedCR.SensorTypeName, cr.SensorTypeName);
                    Assert.Equal(expectedCR.Unit, cr.Unit);
                    Assert.Equal(expectedCR.Value, cr.Value);
                    Assert.Equal(expectedCR.Province, cr.Province);
                    Assert.Equal(expectedCR.City, cr.City);
                    Assert.Equal(expectedCR.IsHistoric, cr.IsHistoric);
                    Assert.Equal(expectedCR.DaysOfMeasure, cr.DaysOfMeasure);
                    Assert.Equal(expectedCR.UtmNord, cr.UtmNord);
                    Assert.Equal(expectedCR.UtmEst, cr.UtmEst);
                    Assert.Equal(expectedCR.Latitude, cr.Latitude);
                    Assert.Equal(expectedCR.Longitude, cr.Longitude);
                    Assert.Equal(expectedCR.Status, cr.Status);
                }

                foreach (var expectedAPD in expectedAPDs)
                {
                    var apd = averageProvinceDatas.FirstOrDefault(x => x.AverageProvinceDataId == expectedAPD.AverageProvinceDataId)!;
                    Assert.NotNull(apd);

                    Assert.Equal(expectedAPD.Province, apd.Province);
                    Assert.Equal(expectedAPD.SensorTypeName, apd.SensorTypeName);
                    Assert.Equal(expectedAPD.AverageValue, apd.AverageValue);
                    Assert.Equal(expectedAPD.Unit, apd.Unit);
                    Assert.Equal(expectedAPD.AverageDaysOfMeasure, apd.AverageDaysOfMeasure);
                    Assert.Equal(expectedAPD.IsExported, apd.IsExported);
                }

                var inputFileText = Encoding.UTF8.GetString(inputFiles.First().Bytes).Replace("\r\n", "\n").Trim();
                var inputFileLines = inputFileText.Split('\n').Select(line => line.Trim()).ToList();

                var outputFileText = Encoding.UTF8.GetString(outputFiles.First().Bytes).Replace("\r\n", "\n").Trim();
                var outputFileLines = outputFileText.Split('\n').Select(line => line.Trim()).ToList();

                foreach (var expectedInputFileLine in expectedInputFileLines)
                    Assert.Contains(expectedInputFileLine, inputFileLines);

                foreach (var expectedOutputFileLine in expectedOutputFileLines)
                    Assert.Contains(expectedOutputFileLine, outputFileLines);
            }
        }

        [Theory]
        [MemberData(nameof(StreamData.InvalidConsistentReadingStreamGenerator), MemberType = typeof(StreamData))]
        public void CreateReadings_ShouldLogErrorsAndUpdate(Stream inputStreamWithErrors, List<string> expectedErrorLines, List<ReadingGetDTO> expectedReadings)
        {
            // Arrange
            _sut.LoggerConfiguration(_outDirectoryPath);
            _integrationTestRepositoryCleanup.Cleanup();
            DirectoryCleanup();
            _integrationTestINFileCreator.CreateINFiles(_inDirectoryPath, 1, inputStreamWithErrors);

            // Act
            _sut.Initialize(_inDirectoryPath);
            _sut.CreateReadings();
            _sut.CreateConsistentReadings();
            _sut.CreateConsistentReadingOutputFiles();
            _sut.ExportConsistentReadings(_outDirectoryPath);

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                Assert.Single(_inputFileRepository.GetByStatusAsync(Status.Error, context));
                var readings = _readingRepository.GetByStatusAsync(Status.Error, context);

                foreach (var expected in expectedReadings)
                {
                    var reading = readings.FirstOrDefault(x => x.ReadingId == expected.ReadingId);
                    Assert.NotNull(reading);

                    Assert.Equal(expected.ReadingId, reading.ReadingId);
                    Assert.Equal(expected.InputFileId, reading.InputFileId);
                    Assert.Equal(expected.SensorId, reading.SensorId);
                    Assert.Equal(expected.SensorTypeName, reading.SensorTypeName);
                    Assert.Equal(expected.Unit, reading.Unit);
                    Assert.Equal(expected.StationId, reading.StationId);
                    Assert.Equal(expected.StationName, reading.StationName);
                    Assert.Equal(expected.Value, reading.Value);
                    Assert.Equal(expected.Province, reading.Province);
                    Assert.Equal(expected.City, reading.City);
                    Assert.Equal(expected.IsHistoric, reading.IsHistoric);
                    Assert.Equal(expected.StartDate, reading.StartDate);
                    Assert.Equal(expected.StopDate, reading.StopDate);
                    Assert.Equal(expected.UtmNord, reading.UtmNord);
                    Assert.Equal(expected.UtmEst, reading.UtmEst);
                    Assert.Equal(expected.Latitude, reading.Latitude);
                    Assert.Equal(expected.Longitude, reading.Longitude);
                    Assert.Equal(expected.Status, reading.Status);
                }
            }

            Log.CloseAndFlush();

            var exportDirectories = Directory.GetDirectories(_outDirectoryPath);
            var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");
            var resultErrorOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

            var errorLines = resultErrorOutputText.Split("\r\n").Select(line => line.Trim()).ToList();

            Assert.Single(exportDirectories);
            Assert.True(File.Exists(resultOutErrorFilePath));
            List<string> test = new List<string>();
            foreach (var expectedErrorLine in expectedErrorLines)
            {
                if (!errorLines.Contains(expectedErrorLine)) test.Add(expectedErrorLine);
                Assert.Contains(expectedErrorLine, errorLines);
            }
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