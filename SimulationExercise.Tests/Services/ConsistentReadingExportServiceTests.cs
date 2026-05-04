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

namespace SimulationExercise.Tests.Services
{
    public class ConsistentReadingExportServiceTests
    {
        private readonly IConsistentReadingExportService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IConsistentReadingRepository> _crRepositoryMock;
        private readonly Mock<IConsistentReadingExportDTOFactory> _crDTOFactoryMock;
        private readonly Mock<IOutputFileService> _outputFileServiceMock;
        private readonly Mock<IOutputFileRepository> _outputFileRepositoryMock;
        private readonly Mock<ILogger<ConsistentReadingExportService>> _loggerMock;

        public ConsistentReadingExportServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _crRepositoryMock = new Mock<IConsistentReadingRepository>();
            _crDTOFactoryMock = new Mock<IConsistentReadingExportDTOFactory>();
            _outputFileServiceMock = new Mock<IOutputFileService>();
            _outputFileRepositoryMock = new Mock<IOutputFileRepository>();
            _loggerMock = new Mock<ILogger<ConsistentReadingExportService>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_crRepositoryMock.Object);
            serviceCollection.AddSingleton(_crDTOFactoryMock.Object);
            serviceCollection.AddSingleton(_outputFileServiceMock.Object);
            serviceCollection.AddSingleton(_outputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new ConsistentReadingExportService
            (
                _contextFactoryMock.Object,
                _crRepositoryMock.Object,
                _crDTOFactoryMock.Object,
                _outputFileServiceMock.Object,
                _outputFileRepositoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public async Task CreateOutputFilesAsync_ShouldCreateOutputFilesAsync()
        {
            // Arrange
            var consistentReadingGetDTOs = new List<ConsistentReadingGetDTO>
            {
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName1", Unit.mg_m3, 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName1", Unit.mg_m3, 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName2", Unit.ng_m3, 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName2", Unit.ng_m3, 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName3", Unit.µg_m3, 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName3", Unit.µg_m3, 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude", false)
            };

            var exportDTOList = new List<ConsistentReadingExportDTO>
            {
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                                                                     
                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                                                                     
                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude")
            };

            var outputFile = new OutputFileInsertDTO("Name", [1, 2, 3], "Extension", "ObjectType", false);

            var outputFileResult = Result<OutputFileInsertDTO>.Ok(outputFile);

            _crRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(consistentReadingGetDTOs);

            _crDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                consistentReadingGetDTOs)).Returns(exportDTOList);

            _outputFileServiceMock.Setup(x => x.CreateOutputFilesAsync(
                exportDTOList)).ReturnsAsync(outputFileResult);

            // Act
            await _sut.CreateOutputFilesAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _crDTOFactoryMock.Verify(x => x.CreateExportDTOList(consistentReadingGetDTOs), Times.Once);

            _outputFileServiceMock.Verify(x => x.CreateOutputFilesAsync(exportDTOList), Times.Once);

            _crRepositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(consistentReadingGetDTOs.Count));
        }

        [Fact]
        public async Task CreateOutputFilesAsync_ShouldLogError_WhenNoNonExportedOutputFilesFound()
        {
            // Arrange
            _crRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(
                new List<ConsistentReadingGetDTO>());

            // Act
            await _sut.CreateOutputFilesAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No non-exported Consistent Readings have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());

            _crDTOFactoryMock.Verify(x => x.CreateExportDTOList(
                It.IsAny<List<ConsistentReadingGetDTO>>()), Times.Never);

            _outputFileServiceMock.Verify(x => x.CreateOutputFilesAsync(
                It.IsAny<List<ConsistentReadingExportDTO>>()), Times.Never);

            _crRepositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateOutputFilesAsync_ShouldLogErrors_WhenOutputFileCreationFails()
        {
            // Arrange
            var consistentReadingGetDTOs = new List<ConsistentReadingGetDTO>
            {
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName1", Unit.mg_m3, 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName1", Unit.mg_m3, 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName2", Unit.ng_m3, 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName2", Unit.ng_m3, 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName3", Unit.µg_m3, 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName3", Unit.µg_m3, 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude", false)
            };

            var exportDTOList = new List<ConsistentReadingExportDTO>
            {
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                                                                     
                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                                                                     
                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude")
            };

            _crRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(consistentReadingGetDTOs);

            _crDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                consistentReadingGetDTOs)).Returns(exportDTOList);

            _outputFileServiceMock.Setup(x => x.CreateOutputFilesAsync(
                exportDTOList)).ReturnsAsync(Result<OutputFileInsertDTO>.Ko(["ERROR"]));

            // Act
            await _sut.CreateOutputFilesAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());

            _crDTOFactoryMock.Verify(x => x.CreateExportDTOList(
                consistentReadingGetDTOs), Times.Once);

            _outputFileServiceMock.Verify(x => x.CreateOutputFilesAsync(
                exportDTOList), Times.Once);

            _crRepositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateOutputFilesAsync_ShouldLogError_WhenExceptionIsThrownAsync()
        {
            // Arrange
            _crRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ThrowsAsync(new Exception("ERROR"));

            // Act
            await _sut.CreateOutputFilesAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());

            _crDTOFactoryMock.Verify(x => x.CreateExportDTOList(
                It.IsAny<List<ConsistentReadingGetDTO>>()), Times.Never);

            _outputFileServiceMock.Verify(x => x.CreateOutputFilesAsync(
                It.IsAny<List<ConsistentReadingExportDTO>>()), Times.Never);

            _crRepositoryMock.Verify(x => x.UpdateAsync(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()),
                Times.Never);
        }

        [Fact]
        public async Task ExportAsync_ShouldLogError_WhenNoNonExportedOutputFileFound()
        {
            // Arrange
            _outputFileRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, "ConsistentReading", It.IsAny<IContext>()))
                .ReturnsAsync(new List<OutputFileGetDTO>());

            // Act
            await _sut.ExportAsync("OutDirectoryPath");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No non-exported Output Files have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());
        }

        [Fact]
        public async Task ExportAsync_ShouldLogError_WhenExceptionIsThrownAsync()
        {
            // Arrange
            _outputFileRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, "ConsistentReading", It.IsAny<IContext>()))
                .ThrowsAsync(new Exception("ERROR"));

            // Act
            await _sut.ExportAsync("OutDirectoryPath");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());
        }
    }
}
