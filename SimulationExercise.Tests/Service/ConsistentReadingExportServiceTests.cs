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
    public class ConsistentReadingExportServiceTests
    {
        private readonly IConsistentReadingExportService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IConsistentReadingRepository> _crRepositoryMock;
        private readonly Mock<IConsistentReadingExportDTOFactory> _crDTOFactoryMock;
        private readonly Mock<IOutputFileService> _outputFileServiceMock;
        private readonly Mock<IOutputFileRepository> _outputFileRepositoryMock;
        private readonly Mock<ILogger<ConsistentReadingExportService>> _loggerMock;

        public ConsistentReadingExportServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _crRepositoryMock = new Mock<IConsistentReadingRepository>();
            _crDTOFactoryMock = new Mock<IConsistentReadingExportDTOFactory>();
            _outputFileServiceMock = new Mock<IOutputFileService>();
            _outputFileRepositoryMock = new Mock<IOutputFileRepository>();
            _loggerMock = new Mock<ILogger<ConsistentReadingExportService>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_crRepositoryMock.Object);
            serviceCollection.AddSingleton(_crDTOFactoryMock.Object);
            serviceCollection.AddSingleton(_outputFileServiceMock.Object);
            serviceCollection.AddSingleton(_outputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new ConsistentReadingExportService
            (
                _contextFactoryMock.Object,
                _crRepositoryMock.Object,
                _crDTOFactoryMock.Object,
                _outputFileServiceMock.Object,
                _outputFileRepositoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public void CreateOutputFiles_ShouldCreateOutputFiles()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogError_WhenNoNonExportedOutputFilesFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogErrors_WhenCRCreationFails()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogError_WhenDTOCreationFails()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Export_ShouldExportConsistentReadings()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Export_ShouldLogError_WhenNoNonExportedOutputFileFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Export_ShouldLogError_WhenOutputFileExportFails()
        {
            throw new NotImplementedException();
        }
    }
}
