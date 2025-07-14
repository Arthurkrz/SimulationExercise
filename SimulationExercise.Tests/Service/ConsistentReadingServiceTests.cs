using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;
using SimulationExercise.Tests.Repository;

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
        private readonly ITestRepositoryCleanup _testRepositoryCleanup;

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

        [Fact]
        public void ProcessReadings_ShouldProcessReadings()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _readingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<ReadingGetDTO>());

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
                Times.Once);

            _readingRepositoryMock.Verify(x => x.Insert(
                It.IsAny<ReadingInsertDTO>(), It.IsAny<IContext>()), 
                Times.Once);
        }

        [Fact]
        public void ProcessReadings_ShouldLogError_WhenNoNewObjectsFound()
        {
            // Arrange
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

        [Fact]
        public void ProcessReadings_ShouldLogError_WhenFailToInsertReading()
        {

        }

        [Fact]
        public void ProcessReadings_ShouldLogError_WhenFailToUpdateReading()
        {

        }
    }
}
