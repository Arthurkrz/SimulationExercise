using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Services;

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

        public ConsistentReadingServiceTests()
        {
            _consistentReadingFactoryMock = new Mock<IConsistentReadingFactory>();
            _consistentReadingInsertDTOFactoryMock = new Mock<IConsistentReadingInsertDTOFactory>();
            _contextFactoryMock = new Mock<IContextFactory>();
            _readingRepositoryMock = new Mock<IReadingRepository>();
            _consistentReadingRepositoryMock = new Mock<IConsistentReadingRepository>();
            _loggerMock = new Mock<ILogger<ConsistentReadingService>>();

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
        }

        [Fact]
        public void ProcessReadings_ShouldProcessReadings()
        {

        }

        [Fact]
        public void ProcessReadings_ShouldLogError_WhenNoNewObjectsFound()
        {

        }
    }
}
