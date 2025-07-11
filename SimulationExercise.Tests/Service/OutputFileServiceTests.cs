using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;
using SimulationExercise.Services.Factories;
using SimulationExercise.Tests.Repository;

namespace SimulationExercise.Tests.Service
{
    public class OutputFileServiceTests
    {
        private readonly IOutputFileService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IConsistentReadingRepository> _consistentReadingRepositoryMock;
        private readonly Mock<IOutputFileRepository> _outputFileRepositoryMock;
        private readonly Mock<IConsistentReadingExportDTOFactory> _consistentReadingExportDTOFactoryMock;
        private readonly Mock<ILogger<OutputFileService>> _loggerMock;
        private readonly ITestRepositoryCleanup _testRepositoryCleanup;


        public OutputFileServiceTests()
        {
            _consistentReadingRepositoryMock = new Mock<IConsistentReadingRepository>();
            _outputFileRepositoryMock = new Mock<IOutputFileRepository>();
            _consistentReadingExportDTOFactoryMock = new Mock<IConsistentReadingExportDTOFactory>();
            _loggerMock = new Mock<ILogger<OutputFileService>>();
            _testRepositoryCleanup = new TestRepositoryCleanup();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingRepositoryMock);
            serviceCollection.AddSingleton(_consistentReadingExportDTOFactoryMock);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new OutputFileService
            (
                _contextFactoryMock.Object,
                _consistentReadingRepositoryMock.Object,
                _outputFileRepositoryMock.Object,
                _consistentReadingExportDTOFactoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldProcessConsistentReadings()
        {
            _testRepositoryCleanup.Cleanup();

            _consistentReadingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<ConsistentReadingGetDTO>());

            // Act
            _sut.ProcessConsistentReadings();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _consistentReadingRepositoryMock.Verify(x => x.Update(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()), 
                Times.Once);

            _consistentReadingRepositoryMock.Verify(x => x.Insert(
                It.IsAny<ConsistentReadingInsertDTO>(), It.IsAny<IContext>()),
                Times.Once);
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenNoNewObjectsFound()
        {
            // Arrange
            _consistentReadingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<ConsistentReadingGetDTO>());

            // Act & Assert
            _sut.ProcessConsistentReadings();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No new Consistent Readings have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenFailToInsert()
        {

        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenFailToUpdate()
        {

        }
    }
}
