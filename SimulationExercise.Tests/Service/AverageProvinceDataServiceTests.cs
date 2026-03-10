using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Service
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
        public void ProcessConsistentReadings_ShouldProcessConsistentReadings()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenNoOutputFileFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogErrors_WhenAPDCreationFails()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenAPDCreationReturnsException()
        {
            throw new NotImplementedException();
        }
    }
}
