using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;
using SimulationExercise.Tests.Repository;

namespace SimulationExercise.Tests.Service
{
    public class ReadingServiceTests
    {
        private readonly IReadingService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IInputFileRepository> _inputFileRepositoryMock;
        private readonly Mock<IReadingImportService> _readingImportServiceMock;
        private readonly Mock<IReadingRepository> _readingRepositoryMock;
        private readonly Mock<IReadingInsertDTOFactory> _readingInsertDTOFactoryMock;
        private readonly Mock<ILogger<ReadingService>> _loggerMock;
        private readonly ITestRepositoryCleanup _testRepositoryCleanup;

        public ReadingServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _inputFileRepositoryMock = new Mock<IInputFileRepository>();
            _readingImportServiceMock = new Mock<IReadingImportService>();
            _readingRepositoryMock = new Mock<IReadingRepository>();
            _readingInsertDTOFactoryMock = new Mock<IReadingInsertDTOFactory>();
            _loggerMock = new Mock<ILogger<ReadingService>>();
            _testRepositoryCleanup = new TestRepositoryCleanup();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_inputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_readingImportServiceMock.Object);
            serviceCollection.AddSingleton(_readingRepositoryMock.Object);
            serviceCollection.AddSingleton(_readingInsertDTOFactoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new ReadingService
            (
                _contextFactoryMock.Object,
                _inputFileRepositoryMock.Object,
                _readingImportServiceMock.Object,
                _readingRepositoryMock.Object,
                _readingInsertDTOFactoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public void ProcessInputFiles_ShouldProcessInputFiles()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<InputFileGetDTO>());

            // Act
            _sut.ProcessInputFiles();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _inputFileRepositoryMock.Verify(x => x.Update(
                It.IsAny<InputFileUpdateDTO>(), It.IsAny<IContext>()), 
                Times.Once);

            _readingRepositoryMock.Verify(x => x.Insert(
                It.IsAny<ReadingInsertDTO>(), It.IsAny<IContext>()), 
                Times.Once);
        }

        [Fact]
        public void ProcessInputFiles_ShouldLogError_IfNoNewObjectsFound()
        {
            // Arrange
            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<InputFileGetDTO>());

            // Act & Assert
            _sut.ProcessInputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No new Input Files have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessInputFiles_ShouldLogError_IfNoReadingsImported()
        {
            // Arrange
            var emptyImportResult = new ImportResult(new List<Reading>(), new List<string> { "Error" });

            _readingRepositoryMock.Setup(x => x.GetByStatus(It.IsAny<Status>(), It.IsAny<IContext>())).Returns(new List<ReadingGetDTO>());

            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                                     .Returns(emptyImportResult);

            // Act & Assert
            _sut.ProcessInputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains(LogMessages.NOREADINGIMPORTED)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessInputFiles_ShouldLogError_WhenFailToInsert()
        {

        }

        [Fact]
        public void ProcessInputFiles_ShouldLogError_WhenFailToUpdate()
        {

        }
    }
}
