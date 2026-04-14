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
using System.Text;

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

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public void CreateOutputFiles_ShouldCallOutputFileCreateMethod()
        {
            // Arrange
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);
            var apdExportDTO = new AverageProvinceDataExportDTO("Province1", "Sensor1", 10, "mg/m³", 20);

            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>()))
                .Returns(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _apdExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<AverageProvinceDataGetDTO>>()))
                .Returns(new List<AverageProvinceDataExportDTO> { apdExportDTO });

            // Act & Assert
            _sut.CreateOutputFilesAsync();

            _outputFileServiceMock.Verify(x => x.CreateOutputFilesAsync<AverageProvinceDataExportDTO>(
                new List<AverageProvinceDataExportDTO> { apdExportDTO }), Times.Once);
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogError_WhenNoAPDsFound()
        {
            // Arrange
            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(false, It.IsAny<IContext>()))
                                  .Returns(new List<AverageProvinceDataGetDTO>());

            // Act & Assert
            _sut.CreateOutputFilesAsync();

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

            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>()))
                .Returns(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _outputFileServiceMock.Setup(x => x.CreateOutputFilesAsync(
                It.IsAny<IList<AverageProvinceDataExportDTO>>()))
                .Returns(Result<OutputFileInsertDTO>.Ko(new List<string> { "ERROR" }));

            _sut.CreateOutputFilesAsync();

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

            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>()))
                .Returns(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _apdExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<AverageProvinceDataGetDTO>>()))
                .Returns(new List<AverageProvinceDataExportDTO> { apdExportDTO });

            _outputFileServiceMock.Setup(x => x.CreateOutputFilesAsync<AverageProvinceDataExportDTO>(
                It.IsAny<IList<AverageProvinceDataExportDTO>>()))
                .Returns(outputFileCreationResult);

            _apdRepositoryMock.Setup(x => x.UpdateAsync(
                It.IsAny<AverageProvinceDataUpdateDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Update failed"));

            // Act & Assert
            _sut.CreateOutputFilesAsync();

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
        public void Export_ShouldCallOutputFileExportMethod()
        {
            // Arrange
            var outputFileText = "Province,SensorTypeName,AverageValue,Unit,AverageDaysOfMeasure\n" +
                                 "Province,SensorTypeName,1,mg_m3,1";

            var outputFileBytes = Encoding.UTF8.GetBytes(outputFileText);
            var outputFileStream = new MemoryStream(outputFileBytes);

            var outputFileGetDTO = new OutputFileGetDTO(1, "Name", outputFileBytes, ".csv", "AverageProvinceData", false);

            _outputFileRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).
                Returns(new List<OutputFileGetDTO> { outputFileGetDTO });

            // Act
            _sut.ExportAsync("OUT");

            // Assert
            _outputFileServiceMock.Verify(x => x.Export<AverageProvinceData>(outputFileGetDTO, outputFileStream), Times.Once);
        }

        [Fact]
        public void Export_ShouldLogError_WhenNoOutputFilesFound()
        {
            // Arrange
            _outputFileRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                true, It.IsAny<IContext>())).
                Returns(new List<OutputFileGetDTO>());

            // Act
            _sut.ExportAsync("OutputFilePath");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No new Output Files have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void Export_ShouldLogError_WhenNoOutDirectoryFound()
        {
            // Act
            _sut.ExportAsync("");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}