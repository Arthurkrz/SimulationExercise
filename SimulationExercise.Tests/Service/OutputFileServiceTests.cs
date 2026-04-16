using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Service
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

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public async Task CreateOutputFilesAsync_ShouldCreateOutputFilesAsync(IList<object> exportDTOs)
        {
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

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public async Task CreateOutputFilesAsync_ShouldLogError_WhenExceptionIsThrownAsync(IList<object> exportDTOs)
        {
            // Arrange
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

        public static IEnumerable<object[]> GetValidObjects()
        {
            yield return new object[]
            {
                new List<ConsistentReadingExportDTO>
                {
                    new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                    new ConsistentReadingExportDTO(1, "SensorTypeName1", "mg_m3", 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                    new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                    new ConsistentReadingExportDTO(1, "SensorTypeName2", "ng_m3", 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                    new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                    new ConsistentReadingExportDTO(1, "SensorTypeName3", "µg_m3", 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude")
                }
            };

            yield return new object[]
            {
                new List<AverageProvinceDataExportDTO>
                {
                    new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "mg_m3", 1),
                    new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "ng_m3", 1),
                    new AverageProvinceDataExportDTO("Province", "SensorTypeName", 1, "µg_m3", 1),
                }
            };
        }
    }
}