using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Infrastructure;
using SimulationExercise.IOC;
using SimulationExercise.Services;
using SimulationExercise.Tests.Integration.ObjectGenerators;
using SimulationExercise.Tests.Integration.Utilities;
using System.Text;

namespace SimulationExercise.Tests.Integration
{
    public class FilePersistanceIntegrationTest
    {
        private readonly IFilePersistanceService _sut;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IContextFactory _contextFactory;

        private readonly IntegrationTestRepositoryCleanup _integrationTestRepositoryCleanup;
        private readonly IntegrationTestINFileCreator _integrationTestINFileCreator;

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

            var connectionString = config.GetConnectionString("DefaultDatabase");

            ServiceCollection services = new ServiceCollection();
            services.InjectServices();
            services.InjectRepositories();
            services.InjectValidators();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            services.AddSingleton<string>(connectionString);
            _serviceProvider = services.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<IFilePersistanceService>();
            _contextFactory = _serviceProvider.GetRequiredService<IContextFactory>();
            _inputFileRepository = _serviceProvider.GetRequiredService<IInputFileRepository>();
            _readingRepository = _serviceProvider.GetRequiredService<IReadingRepository>();
            _consistentReadingRepository = _serviceProvider.GetRequiredService<IConsistentReadingRepository>();
            _outputFileRepository = _serviceProvider.GetRequiredService<IOutputFileRepository>();

            _basePath = Path.Combine(Path.GetTempPath(), "SimulationExerciseTests.Integration");
            _inDirectoryPath = Path.Combine(_basePath, "INTest");
            _outDirectoryPath = Path.Combine(_basePath, "OUTTest");

            _integrationTestRepositoryCleanup = new IntegrationTestRepositoryCleanup();
            _integrationTestINFileCreator = new IntegrationTestINFileCreator();
        }

        [Theory]
        [MemberData(nameof(StreamData.ValidStreamGenerator), MemberType = typeof(StreamData))]
        public void AllPersistanceSteps_ShouldProcessExportAndPersistInDatabase(string inputText,
                                                                                List<InputFileGetDTO> expectedInputFiles,
                                                                                List<ReadingGetDTO> expectedReadings,
                                                                                List<ConsistentReadingGetDTO> expectedCRs,
                                                                                List<OutputFileGetDTO> expectedOutputFiles)
        {
            // Arrange
            _sut.LoggerConfiguration(_outDirectoryPath);
            _integrationTestRepositoryCleanup.Cleanup();
            DirectoryCleanup();
            _integrationTestINFileCreator.CreateINFiles(_inDirectoryPath, 1, inputText);

            // Act
            _sut.Initialize(_inDirectoryPath);
            _sut.CreateReadings();
            _sut.CreateConsistentReadings();
            _sut.CreateOutputFiles();

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var inputFiles = _inputFileRepository.GetByStatus(Status.Success, context);
                var readings = _readingRepository.GetByStatus(Status.Success, context);
                var consistentReadings = _consistentReadingRepository.GetByStatus(Status.Success, context);
                var outputFiles = _outputFileRepository.GetByStatus(Status.Success, context);

                inputFiles.Should().BeEquivalentTo(expectedInputFiles);
                readings.Should().BeEquivalentTo(expectedReadings);
                consistentReadings.Should().BeEquivalentTo(expectedCRs);
                outputFiles.Should().BeEquivalentTo(expectedOutputFiles);
            }
        }

        [Theory]
        [MemberData(nameof(StreamData.InvalidStreamGenerator), MemberType = typeof(StreamData))]
        public void CreateReadings_ShouldLogErrorsAndUpdate(string inputTextWithErrors, string expectedErrors, List<InputFileGetDTO> expectedInputFiles)
        {
            // Arrange
            _sut.LoggerConfiguration(_outDirectoryPath);
            _integrationTestRepositoryCleanup.Cleanup();
            DirectoryCleanup();
            _integrationTestINFileCreator.CreateINFiles(_inDirectoryPath, 1, inputTextWithErrors);

            // Act
            _sut.Initialize(_inDirectoryPath);
            _sut.CreateReadings();
            _sut.CreateConsistentReadings();
            _sut.CreateOutputFiles();

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var inputFiles = _inputFileRepository.GetByStatus(Status.Error, context);
                inputFiles.Should().BeEquivalentTo(expectedInputFiles);

                var exportDirectories = Directory.GetDirectories(_outDirectoryPath);
                var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "Errors.log");
                var resultErrorOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

                Assert.Single(exportDirectories);
                Assert.True(File.Exists(resultOutErrorFilePath));
                Assert.Equal(expectedErrors.Trim(), resultErrorOutputText);
            }
        }

        [Fact]
        public void CreateConsistentReadings_ShouldCreateNewConsistentReadings()
        {
            // Arrange
            var inputFileText = $@"IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location
1,SensorTypeName,ng/m³,1,StationName,1,Province,City,S,{DateTime.Now.AddDays(-1).Date},{DateTime.Now.Date},1,1,Latitude,Longitude,POINT (Latitude Longitude)
1,SensorTypeName,mg/m³,1,StationName,1,Province,City,N,{DateTime.Now.AddDays(-1).Date},{DateTime.Now.Date},1,1,Latitude,Longitude,POINT (Latitude Longitude)";

            var expectedConsistentReadings = new List<ConsistentReadingGetDTO>
            {
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                new ConsistentReadingGetDTO(2, 2, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New)
            };

            _sut.LoggerConfiguration(_outDirectoryPath);
            _integrationTestRepositoryCleanup.Cleanup();
            DirectoryCleanup();
            _integrationTestINFileCreator.CreateINFiles(_inDirectoryPath, 1, inputFileText);

            // Act
            _sut.Initialize(_inDirectoryPath);
            _sut.CreateReadings();
            _sut.CreateConsistentReadings();

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var consistentReadings = _consistentReadingRepository.GetByStatus(Status.New, context);
                consistentReadings.Should().BeEquivalentTo(expectedConsistentReadings);
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