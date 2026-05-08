using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;
using CSVReader.Core.Enum;
using CSVReader.Services;

namespace CSVReader.Tests.Services
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
        public async Task CreateOutputFilesAsync_ShouldCallOutputFileCreateMethodAsync()
        {
            // Arrange
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);
            var apdExportDTO = new AverageProvinceDataExportDTO("Province1", "Sensor1", 10, "mg/m³", 20);

            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>()))
                .ReturnsAsync(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _apdExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<AverageProvinceDataGetDTO>>()))
                .Returns(new List<AverageProvinceDataExportDTO> { apdExportDTO });

            // Act & Assert
            await _sut.CreateOutputFilesAsync();

            _outputFileServiceMock.Verify(x => x.CreateOutputFilesAsync(
                new List<AverageProvinceDataExportDTO> { apdExportDTO }), Times.Once);
        }

        [Fact]
        public async Task CreateOutputFilesAsync_ShouldLogError_WhenNoAPDsFoundAsync()
        {
            // Arrange
            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(false, It.IsAny<IContext>()))
                .ReturnsAsync(new List<AverageProvinceDataGetDTO>());

            // Act
            await _sut.CreateOutputFilesAsync();

            // Assert
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
        public async Task CreateOutputFilesAsync_ShouldLogErrors_WhenOutputFileCreationFailsAsync()
        {
            // Arrange
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);

            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>()))
                .ReturnsAsync(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _outputFileServiceMock.Setup(x => x.CreateOutputFilesAsync(
                It.IsAny<IList<AverageProvinceDataExportDTO>>()))
                .ReturnsAsync(Result<OutputFileInsertDTO>.Ko(new List<string> { "ERROR" }));

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
                Times.Once);
        }

        [Fact]
        public async Task CreateOutputFilesAsync_ShouldLogError_WhenUpdateFailsAsync()
        {
            // Arrange
            var apdGetDTO = new AverageProvinceDataGetDTO(1, "Province1", "Sensor1", 10, Unit.mg_m3, 20, false);
            var apdExportDTO = new AverageProvinceDataExportDTO("Province1", "Sensor1", 10, "mg/m³", 20);
            var outputFileCreationResult = Result<OutputFileInsertDTO>.Ok(new OutputFileInsertDTO("AverageProvinceDataExportDTO", new byte[] { 1, 2, 3 }, ".csv", "AverageProvinceData", false));

            _apdRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>()))
                .ReturnsAsync(new List<AverageProvinceDataGetDTO> { apdGetDTO });

            _apdExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<AverageProvinceDataGetDTO>>()))
                .Returns(new List<AverageProvinceDataExportDTO> { apdExportDTO });

            _outputFileServiceMock.Setup(x => x.CreateOutputFilesAsync(
                It.IsAny<IList<AverageProvinceDataExportDTO>>()))
                .ReturnsAsync(outputFileCreationResult);

            _apdRepositoryMock.Setup(x => x.UpdateAsync(
                It.IsAny<AverageProvinceDataUpdateDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Update failed"));

            // Act
            await _sut.CreateOutputFilesAsync();

            // Assert
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
        public async Task ExportAsync_ShouldLogError_WhenNoOutputFilesFoundAsync()
        {
            // Arrange
            _outputFileRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, "AverageProvinceData", It.IsAny<IContext>())).
                ReturnsAsync(new List<OutputFileGetDTO>());

            // Act
            await _sut.ExportAsync("OutputFilePath");

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
        public async Task ExportAsync_ShouldLogError_WhenNoOutDirectoryFoundAsync()
        {
            // Act
            await _sut.ExportAsync("OutDirectoryPath");

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