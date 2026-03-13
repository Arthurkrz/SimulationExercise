using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Service
{
    public class AverageProvinceDataExportServiceTests
    {
        private readonly IAverageProvinceDataExportService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IAverageProvinceDataRepository> _apdRepositoryMock;
        private readonly Mock<IAverageProvinceDataExportDTOFactory> _apdExportDTOFactoryMock;
        private readonly Mock<IOutputFileService> _outputFileServiceMock;
        private readonly Mock<IOutputFileRepository> _outputFileRepositoryMock;
        private readonly Mock<ILogger<AverageProvinceDataExportService>> _loggerMock;

        private readonly string _basePath;
        private readonly string _outDirectoryPath;

        public AverageProvinceDataExportServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _apdRepositoryMock = new Mock<IAverageProvinceDataRepository>();
            _apdExportDTOFactoryMock = new Mock<IAverageProvinceDataExportDTOFactory>();
            _outputFileServiceMock = new Mock<IOutputFileService>();
            _outputFileRepositoryMock = new Mock<IOutputFileRepository>();
            _loggerMock = new Mock<ILogger<AverageProvinceDataExportService>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_apdRepositoryMock.Object);
            serviceCollection.AddSingleton(_apdExportDTOFactoryMock.Object);
            serviceCollection.AddSingleton(_outputFileServiceMock.Object);
            serviceCollection.AddSingleton(_outputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new AverageProvinceDataExportService
            (
                _contextFactoryMock.Object,
                _apdRepositoryMock.Object,
                _apdExportDTOFactoryMock.Object,
                _outputFileServiceMock.Object,
                _outputFileRepositoryMock.Object,
                _loggerMock.Object
            );

            _basePath = Path.Combine(Path.GetTempPath(), "SimulationExerciseTests");
            _outDirectoryPath = Path.Combine(_basePath, "OUT");

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public void CreateOutputFiles_ShouldCreateOutputFiles()
        {
            // Arrange
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);
            var apdExportDTO = new AverageProvinceDataExportDTO("Province1", "Sensor1", 10, "mg/m³", 20);

            _apdRepositoryMock.Setup(x => x.GetByIsExported(
                false, It.IsAny<IContext>()))
                .Returns(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _apdExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<AverageProvinceDataGetDTO>>()))
                .Returns(new List<AverageProvinceDataExportDTO> { apdExportDTO });

            // Act & Assert
            _sut.CreateOutputFiles();

            _outputFileServiceMock.Verify(x => x.CreateOutputFiles<AverageProvinceDataExportDTO>(
                new List<AverageProvinceDataExportDTO> { apdExportDTO }), Times.Once);
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogError_WhenNoAPDsFound()
        {
            // Arrange
            _apdRepositoryMock.Setup(x => x.GetByIsExported(false, It.IsAny<IContext>()))
                                  .Returns(new List<AverageProvinceDataGetDTO>());

            // Act & Assert
            _sut.CreateOutputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No non-exported Average Province Datas have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogErrors_WhenOutputFileCreationFails()
        {
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);

            _apdRepositoryMock.Setup(x => x.GetByIsExported(
                false, It.IsAny<IContext>()))
                .Returns(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _outputFileServiceMock.Setup(x => x.CreateOutputFiles(
                It.IsAny<IList<AverageProvinceDataExportDTO>>()))
                .Returns(Result<OutputFileInsertDTO>.Ko(new List<string> { "ERROR" }));

            _sut.CreateOutputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogError_WhenUpdateFails()
        {
            // Arrange
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);
            var apdExportDTO = new AverageProvinceDataExportDTO("Province1", "Sensor1", 10, "mg/m³", 20);
            var outputFileCreationResult = Result<OutputFileInsertDTO>.Ok(new OutputFileInsertDTO("AverageProvinceDataExportDTO", new byte[] { 1, 2, 3 }, ".csv", "AverageProvinceData", false));

            _apdRepositoryMock.Setup(x => x.GetByIsExported(
                false, It.IsAny<IContext>()))
                .Returns(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _apdExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<AverageProvinceDataGetDTO>>()))
                .Returns(new List<AverageProvinceDataExportDTO> { apdExportDTO });

            _outputFileServiceMock.Setup(x => x.CreateOutputFiles<AverageProvinceDataExportDTO>(
                It.IsAny<IList<AverageProvinceDataExportDTO>>()))
                .Returns(outputFileCreationResult);

            _apdRepositoryMock.Setup(x => x.Update(
                It.IsAny<AverageProvinceDataUpdateDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Update failed"));

            // Act & Assert
            _sut.CreateOutputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Update failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void Export_ShouldCreateNotEmptyFile()
        {
            // Arrange
            DirectoryCleanup();

            var outFilePath = Path.Combine(_outDirectoryPath, "");
            var outputFileGetDTO = new OutputFileGetDTO(1, "Name", new byte[] { 1, 2, 3 }, ".csv", "AverageProvinceData", false);

            _outputFileRepositoryMock.Setup(x => x.GetByIsExported(
                false, It.IsAny<IContext>())).
                Returns(new List<OutputFileGetDTO> { outputFileGetDTO });

            Stream outputStream = new MemoryStream();

            // Act
            _sut.Export(_outDirectoryPath);

            // Assert
            var outFiles = Directory.GetFiles(_outDirectoryPath);
            Assert.Single(outFiles);
            Assert.NotEmpty(outFiles.First());
        }

        [Fact]
        public void Export_ShouldLogError_WhenNoOutputFilesFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Export_ShouldLogError_WhenNoOutDirectoryFound()
        {
            throw new NotImplementedException();
        }

        private void DirectoryCleanup()
        {
            if (Directory.Exists(_outDirectoryPath)) Directory.Delete(_outDirectoryPath, true);
            Directory.CreateDirectory(_outDirectoryPath);
        }
    }
}