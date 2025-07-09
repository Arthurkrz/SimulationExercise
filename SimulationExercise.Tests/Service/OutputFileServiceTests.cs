using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Service
{
    public class OutputFileServiceTests
    {
        private readonly IOutputFileService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IConsistentReadingRepository> _consistentReadingRepositoryMock;
        private readonly Mock<ILogger<OutputFileService>> _loggerMock;

        public OutputFileServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _consistentReadingRepositoryMock = new Mock<IConsistentReadingRepository>();
            _loggerMock = new Mock<ILogger<OutputFileService>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingRepositoryMock);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new OutputFileService
            (
                _contextFactoryMock.Object,
                _consistentReadingRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldProcessConsistentReadings()
        {

        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenNoNewObjectsFound()
        {

        }
    }
}
