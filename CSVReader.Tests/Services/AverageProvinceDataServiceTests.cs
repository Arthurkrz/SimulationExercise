using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;
using CSVReader.Core.Enum;
using CSVReader.Services;

namespace CSVReader.Tests.Services
{
    public class AverageProvinceDataServiceTests
    {
        private readonly IAverageProvinceDataService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IAverageProvinceDataRepository> _apdRepositoryMock;
        private readonly Mock<IAverageProvinceDataFactory> _apdFactoryMock;
        private readonly Mock<IConsistentReadingRepository> _consistentReadingRepositoryMock;
        private readonly Mock<ILogger<AverageProvinceDataService>> _loggerMock;

        public AverageProvinceDataServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _apdRepositoryMock = new Mock<IAverageProvinceDataRepository>();
            _apdFactoryMock = new Mock<IAverageProvinceDataFactory>();
            _consistentReadingRepositoryMock = new Mock<IConsistentReadingRepository>();
            _loggerMock = new Mock<ILogger<AverageProvinceDataService>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_apdRepositoryMock.Object);
            serviceCollection.AddSingleton(_apdFactoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new AverageProvinceDataService
            (
                _contextFactoryMock.Object,
                _apdRepositoryMock.Object,
                _apdFactoryMock.Object,
                _consistentReadingRepositoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public async Task ProcessConsistentReadingsAsync_ShouldProcessConsistentReadingsAsync()
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

            var apdCreationResults = new List<Result<AverageProvinceData>>
            {
                Result<AverageProvinceData>.Ok(new AverageProvinceData("Province1", "SensorTypeName1", 1, Unit.mg_m3, 1)),
                Result<AverageProvinceData>.Ok(new AverageProvinceData("Province2", "SensorTypeName2", 1, Unit.ng_m3, 1)),
                Result<AverageProvinceData>.Ok(new AverageProvinceData("Province3", "SensorTypeName3", 1, Unit.µg_m3, 1))
            };

            _consistentReadingRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(consistentReadingGetDTOs);

            _apdFactoryMock.Setup(x => x.CreateAverageProvinceData(
                It.IsAny<List<ConsistentReading>>())).Returns(apdCreationResults);

            // Act
            await _sut.ProcessConsistentReadingsAsync();

            // Assert
            _apdRepositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<AverageProvinceDataInsertDTO>(), 
                It.IsAny<IContext>()), Times.Exactly(3));

            _contextFactoryMock.Verify(x => x.Create(),
                Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessConsistentReadingsAsync_ShouldLogError_WhenNoNonExportedCRsFoundAsync()
        {
            // Arrange
            _consistentReadingRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(new List<ConsistentReadingGetDTO>());

            // Act
            await _sut.ProcessConsistentReadingsAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No non-exported ConsistentReadings have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once());
        }

        [Fact]
        public async Task ProcessConsistentReadingsAsync_ShouldLogErrors_WhenAPDCreationFailsAsync()
        {
            // Arrange
            var consistentReadinGetDTOs = new List<ConsistentReadingGetDTO>
            {
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName1", Unit.mg_m3, 1, "Province1", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName1", Unit.mg_m3, 1, "Province1", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName2", Unit.ng_m3, 1, "Province2", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName2", Unit.ng_m3, 1, "Province2", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName3", Unit.µg_m3, 1, "Province3", "City", true, 1, 1, 1, "Latitude", "Longitude", false),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName3", Unit.µg_m3, 1, "Province3", "City", false, 1, 1, 1, "Latitude", "Longitude", false)
            };

            var failedCreation = new List<Result<AverageProvinceData>>
            { Result<AverageProvinceData>.Ko(["ERROR"]) };

            _consistentReadingRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(consistentReadinGetDTOs);

            _apdFactoryMock.Setup(x => x.CreateAverageProvinceData(
                It.IsAny<List<ConsistentReading>>())).Returns(failedCreation);

            // Act
            await _sut.ProcessConsistentReadingsAsync();

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

            _apdRepositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<AverageProvinceDataInsertDTO>(),
                It.IsAny<IContext>()), Times.Never());
        }

        [Fact]
        public void ProcessConsistentReadingsAsync_ShouldLogError_WhenAPDCreationReturnsExceptionAsync()
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

            _consistentReadingRepositoryMock.Setup(x => x.GetByIsExportedAsync(
                false, It.IsAny<IContext>())).ReturnsAsync(consistentReadingGetDTOs);

            _apdFactoryMock.Setup(x => x.CreateAverageProvinceData(
                It.IsAny<List<ConsistentReading>>())).Throws(new Exception("ERROR"));

            // Act
            _sut.ProcessConsistentReadingsAsync();

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

            _apdRepositoryMock.Verify(x => x.InsertAsync(
                It.IsAny<AverageProvinceDataInsertDTO>(),
                It.IsAny<IContext>()), Times.Never());
        }
    }
}
