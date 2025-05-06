using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using SimulationExercise.IOC;
using SimulationExercise.Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SimulationExercise.Tests
{
    public class FileProcessingServiceTests
    {
        private readonly IFileProcessingService _sut;
        private readonly Mock<ILogger<FileProcessingService>> _loggerMock;
        private readonly Mock<IValidator> _validatorMock;
        private readonly Mock<IReadingImportService> _readingImportServiceMock;
        private readonly Mock<IConsistentReadingFactory> _consistentReadingFactoryMock;
        private readonly Mock<IProvinceDataListFactory> _provinceDataListFactoryMock;
        private readonly Mock<IAverageProvinceDataFactory> _averageProvinceDataFactoryMock;
        private readonly Mock<IAverageProvinceDataExportService> _averageProvinceDataExportServiceMock;

        public FileProcessingServiceTests()
        {
            _loggerMock = new Mock<ILogger<FileProcessingService>>();
            _validatorMock = new Mock<IValidator>();
            _readingImportServiceMock = new Mock<IReadingImportService>();
            _consistentReadingFactoryMock = new Mock<IConsistentReadingFactory>();
            _provinceDataListFactoryMock = new Mock<IProvinceDataListFactory>();
            _averageProvinceDataFactoryMock = new Mock<IAverageProvinceDataFactory>();
            _averageProvinceDataExportServiceMock = new Mock<IAverageProvinceDataExportService>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_loggerMock.Object);
            serviceCollection.AddSingleton(_validatorMock.Object);
            serviceCollection.AddSingleton(_readingImportServiceMock.Object);
            serviceCollection.AddSingleton(_consistentReadingFactoryMock.Object);
            serviceCollection.AddSingleton(_provinceDataListFactoryMock.Object);
            serviceCollection.AddSingleton(_averageProvinceDataFactoryMock.Object);
            serviceCollection.AddSingleton(_averageProvinceDataExportServiceMock.Object);

            _sut = new FileProcessingService(
                _readingImportServiceMock.Object,
                _consistentReadingFactoryMock.Object,
                _provinceDataListFactoryMock.Object,
                _averageProvinceDataFactoryMock.Object,
                _averageProvinceDataExportServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void ProcessFile_ShouldProcessFile()
        {
            // Arrange
            string outDirectoryTestPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest";

            // Act
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", outDirectoryTestPath);
            var files = Directory.GetFiles(outDirectoryTestPath);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            Assert.NotEmpty(files);
        }

        [Fact]
        public void ProcessFile_ShouldCreateDirectory_WhenNoInDirectoryFound()
        {
            // Act
            string nonExistentInDirectoryTestPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\NonExistentINDirectoryTest";
            _sut.ProcessFile(nonExistentInDirectoryTestPath, "OutputPath");
            
            // Assert
            Assert.True(Directory.Exists(nonExistentInDirectoryTestPath));

            // Teardown
            Directory.Delete(nonExistentInDirectoryTestPath, true);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoCSVFilesFound()
        {
            // Act & Assert
            string emptyInDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\EmptyINDirectory";
            _sut.ProcessFile(emptyInDirectoryPath, "");
            _loggerMock.Verify(x => x.LogError(It.IsAny<string>()), Times.Once);

            // Teardown
            Directory.Delete(emptyInDirectoryPath, true);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoReadingsImported()
        {
            // Arrange
            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                .Returns(new ImportResult(new List<Reading>(), new List<string> { "ERROR" }));

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest"));
            Assert.Equal("No readings have been imported!", exception.Message);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("No readings have been imported!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoConsistentReadingsCreated()
        {
            // Arrange
            Result<ConsistentReading> result = 
                Result<ConsistentReading>.Ko(new List<string> { "ERROR" });

            _consistentReadingFactoryMock.Setup(x => x
                                         .CreateConsistentReading(It.IsAny<Reading>()))
                                         .Returns(result);

            // Act & Assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest"));
            Assert.Equal("No consistent readings have been created!", exception.Message);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("ERROR")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("No consistent readings have been created!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoProvinceDataCreated()
        {
            // Arrange
            _provinceDataListFactoryMock.Setup(x => x.CreateProvinceDataList(It.IsAny<List<ConsistentReading>>()))
                                        .Returns(new List<ProvinceData>());

            // Act & Assert
            var exception = Assert.Throws<NullReferenceException>(() => _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest"));
            Assert.Equal("No province data have been created!", exception.Message);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("No province data have been created!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoAverageProvinceDataCreated()
        {
            // Arrange
            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                                     .Returns(It.IsAny<ImportResult>);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(It.IsAny<Reading>()))
                                         .Returns(It.IsAny<Result<ConsistentReading>>);

            _provinceDataListFactoryMock.Setup(x => x.CreateProvinceDataList(It.IsAny<IList<ConsistentReading>>()))
                                        .Returns(It.IsAny<IList<ProvinceData>>);

            _averageProvinceDataFactoryMock.Setup(x => x.CreateAverageProvinceData(It.IsAny<ProvinceData>()))
                                           .Returns(It.IsAny<Result<AverageProvinceData>>);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest"));
            Assert.Equal("No average province data have been created!", exception.Message);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("No average province data have been created!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
