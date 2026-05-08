using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Services;

namespace CSVReader.Tests.Services
{
    public class OutputFileServiceTests
    {
        private readonly IOutputFileService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IOutputFileRepository> _outputFileRepositoryMock;
        private readonly Mock<ILogger<OutputFileService>> _loggerMock;

        public OutputFileServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _outputFileRepositoryMock = new Mock<IOutputFileRepository>();
            _loggerMock = new Mock<ILogger<OutputFileService>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_outputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new OutputFileService
            (
                _contextFactoryMock.Object,
                _outputFileRepositoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public async Task CreateOutputFilesAsyncWithConsistentReading_ShouldCreateOutputFilesAsync()
        {
            // Arrange
            var exportDTOs = new List<ConsistentReadingExportDTO>
            {
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude")
            };

            // Act
            var result = await _sut.CreateOutputFilesAsync(exportDTOs);

            // Assert
            Assert.True(result.Success);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _outputFileRepositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<OutputFileInsertDTO>(), It.IsAny<IContext>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateOutputFilesAsyncWithAverageProvinceData_ShouldCreateOutputFilesAsync()
        {
            // Arrange
            var exportDTOs = new List<AverageProvinceDataExportDTO>
            {
                new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "mg_m3", 1),
                new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "ng_m3", 1),
                new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "µg_m3", 1)
            };

            // Act
            var result = await _sut.CreateOutputFilesAsync(exportDTOs);

            // Assert
            Assert.True(result.Success);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _outputFileRepositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<OutputFileInsertDTO>(), It.IsAny<IContext>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateOutputFilesAsyncWithConsistentReading_ShouldLogError_WhenExceptionIsThrownAsync()
        {
            // Arrange
            var exportDTOs = new List<ConsistentReadingExportDTO>
            {
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude")
            };

            _outputFileRepositoryMock.Setup(x => x.InsertAsync(
                It.IsAny<OutputFileInsertDTO>(), It.IsAny<IContext>()))
                .ThrowsAsync(new Exception("ERROR"));

            // Act
            await _sut.CreateOutputFilesAsync(exportDTOs);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateOutputFilesAsyncWithAverageProvinceData_ShouldLogError_WhenExceptionIsThrownAsync()
        {
            // Arrange
            var exportDTOs = new List<AverageProvinceDataExportDTO>
            {
                new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "mg_m3", 1),
                new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "ng_m3", 1),
                new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "µg_m3", 1)
            };

            _outputFileRepositoryMock.Setup(x => x.InsertAsync(
                It.IsAny<OutputFileInsertDTO>(), It.IsAny<IContext>()))
                .ThrowsAsync(new Exception("ERROR"));

            // Act
            await _sut.CreateOutputFilesAsync(exportDTOs);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}