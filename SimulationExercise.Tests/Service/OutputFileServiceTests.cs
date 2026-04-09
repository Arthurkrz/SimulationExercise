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
        public void CreateOutputFiles_ShouldCreateOutputFiles(IList<object> exportDTOs)
        {
            // Act
            _sut.CreateOutputFilesAsync(exportDTOs);

            // Assert
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
        public void CreateOutputFiles_ShouldLogError_WhenNoNonExportedOutputFilesFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogErrors_WhenOutputFileCreationFails()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CreateOutputFiles_ShouldLogError_WhenDTOCreationFails()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Export_ShouldCallOutputFileExportMethod()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<object[]> GetValidObjects()
        {
            yield return new object[]
            {

            };

            yield return new object[]
            {

            };
        }
    }
}