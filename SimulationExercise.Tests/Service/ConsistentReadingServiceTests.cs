using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;
using SimulationExercise.Tests.Utilities;

namespace SimulationExercise.Tests.Service
{
    public class ConsistentReadingServiceTests
    {
        private readonly IConsistentReadingService _sut;
        private readonly Mock<IConsistentReadingFactory> _consistentReadingFactoryMock;
        private readonly Mock<IConsistentReadingInsertDTOFactory> _consistentReadingInsertDTOFactoryMock;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IReadingRepository> _readingRepositoryMock;
        private readonly Mock<IConsistentReadingRepository> _consistentReadingRepositoryMock;
        private readonly Mock<ILogger<ConsistentReadingService>> _loggerMock;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;

        public ConsistentReadingServiceTests()
        {
            _consistentReadingFactoryMock = new Mock<IConsistentReadingFactory>();
            _consistentReadingInsertDTOFactoryMock = new Mock<IConsistentReadingInsertDTOFactory>();
            _contextFactoryMock = new Mock<IContextFactory>();
            _readingRepositoryMock = new Mock<IReadingRepository>();
            _consistentReadingRepositoryMock = new Mock<IConsistentReadingRepository>();
            _loggerMock = new Mock<ILogger<ConsistentReadingService>>();
            _testRepositoryCleanup = new TestRepositoryCleanup();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_consistentReadingFactoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingInsertDTOFactoryMock.Object);
            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_readingRepositoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new ConsistentReadingService
            (
                _consistentReadingFactoryMock.Object,
                _consistentReadingInsertDTOFactoryMock.Object,
                _contextFactoryMock.Object,
                _readingRepositoryMock.Object,
                _consistentReadingRepositoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessReadings_ShouldProcessReadings(List<ReadingGetDTO> readingDTOs, Result<ConsistentReading> creationResult, ConsistentReadingInsertDTO crInsertDTO)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _readingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(readingDTOs);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(
                It.IsAny<Reading>())).Returns(creationResult);

            _consistentReadingInsertDTOFactoryMock.Setup(x => x.CreateConsistentReadingInsertDTO(
                It.IsAny<ConsistentReading>(), It.IsAny<long>()))
                .Returns(crInsertDTO);

            // Act
            _sut.ProcessReadings();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _readingRepositoryMock.Verify(x => x.Update(
                It.IsAny<ReadingUpdateDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(6));

            _consistentReadingRepositoryMock.Verify(x => x.Insert(
                It.IsAny<ConsistentReadingInsertDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(6));
        }

        [Theory]
        [MemberData(nameof(GetInvalidObjects))]
        public void ProcessReadings_ShouldLogErrors_WhenInvalidReadings(List<ReadingGetDTO> readingDTOs, Result<ConsistentReading> creationResult)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _readingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(readingDTOs);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(
                It.IsAny<Reading>())).Returns(creationResult);

            // Act
            _sut.ProcessReadings();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Sensor ID less or equal to 0.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Null or empty sensor name.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unit not supported.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Station ID less or equal to 0.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Null or empty station name.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Negative value.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Null or empty province name.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Null or empty city name.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Start date is before the possible minimum.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Stop date is before start date.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("UTMNord less or equal to 0.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("UTMEst less or equal to 0.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Null or empty latitude.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Null or empty longitude.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));
        }

        [Fact]
        public void ProcessReadings_ShouldLogError_WhenNoNewObjectsFound()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _readingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<ReadingGetDTO>());

            // Act & Assert
            _sut.ProcessReadings();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No new Readings have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessReadings_ShouldLogError_WhenFailToInsert(List<ReadingGetDTO> readingDTOs, Result<ConsistentReading> creationResult, ConsistentReadingInsertDTO crInsertDTO)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _readingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(readingDTOs);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(
                It.IsAny<Reading>())).Returns(creationResult);

            _consistentReadingInsertDTOFactoryMock.Setup(x => x.CreateConsistentReadingInsertDTO(
                It.IsAny<ConsistentReading>(), It.IsAny<long>()))
                .Returns(crInsertDTO);

            _consistentReadingRepositoryMock.Setup(x => x.Insert(
                It.IsAny<ConsistentReadingInsertDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Insert failed"));

            // Act & Assert
            _sut.ProcessReadings();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Insert failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessReadings_ShouldLogError_WhenFailToUpdate(List<ReadingGetDTO> readingDTOs, Result<ConsistentReading> creationResult, ConsistentReadingInsertDTO crInsertDTO)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _readingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(readingDTOs);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(
                It.IsAny<Reading>())).Returns(creationResult);

            _consistentReadingInsertDTOFactoryMock.Setup(x => x.CreateConsistentReadingInsertDTO(
                It.IsAny<ConsistentReading>(), It.IsAny<long>()))
                .Returns(crInsertDTO);

            _readingRepositoryMock.Setup(x => x.Update(
                It.IsAny<ReadingUpdateDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Update failed"));

            // Act & Assert
            _sut.ProcessReadings();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Update failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(6));
        }

        public static IEnumerable<object[]> GetValidObjects()
        {
            var consistentReading = new ConsistentReading(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, "Latitude", "Longitude");

            yield return new object[]
            {
                new List<ReadingGetDTO>
                {
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", false, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),

                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", false, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),

                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", false, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                },

                Result<ConsistentReading>.Ok(consistentReading),

                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New)
            };
        }

        public static IEnumerable<object[]> GetInvalidObjects()
        {
            var crCreationErrors = new List<string>
            {
                "Sensor ID less or equal to 0.",
                "Null or empty sensor name.",
                "Unit not supported.",
                "Station ID less or equal to 0.",
                "Null or empty station name.",
                "Negative value.",
                "Null or empty province name.",
                "Null or empty city name.",
                "Start date is before the possible minimum.",
                "Stop date is before start date.",
                "UTMNord less or equal to 0.",
                "UTMEst less or equal to 0.",
                "Null or empty latitude.",
                "Null or empty longitude."
            };

            yield return new object[]
            {
                new List<ReadingGetDTO>
                {
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", false, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),

                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", false, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),

                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                    new ReadingGetDTO(1, 1, 1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", false, DateTime.Now.AddYears(-2).Date, DateTime.Now.AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.New),
                },

                Result<ConsistentReading>.Ko(crCreationErrors)
            };
        }
    }
}