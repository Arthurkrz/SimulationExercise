using System.Text;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.IOC;
using SimulationExercise.Services;
using Xunit;
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

            if (!Directory.Exists(outDirectoryTestPath)) Directory.CreateDirectory(outDirectoryTestPath);

            Reading reading = new Reading(123, "Sensor", "ng/m³", 123, "Station", 123, "Province", "City", true, DateTime.Now, null, 123, 123, "Latitude", "Longitude");
            ConsistentReading consistentReading = new ConsistentReading(123, "Sensor", Core.Enum.Unit.ng_m3, 123, "Province", "City", true, 123, 123, "Latitude", "Longitude");
            IList<ConsistentReading> consistentReadings = new List<ConsistentReading> { consistentReading };
            ProvinceData provinceData = new ProvinceData("Province", "Sensor", consistentReadings);
            AverageProvinceData averageProvinceData = new AverageProvinceData("Province", "Sensor", 123, Core.Enum.Unit.ng_m3, 123);
            ImportResult importResult = new ImportResult(new List<Reading> { reading }, new List<string>());

            Result<ConsistentReading> resultConsistentReadingCreation = Result<ConsistentReading>.Ok(consistentReading);
            Result<AverageProvinceData> resultAverageProvinceDataCreation = Result<AverageProvinceData>.Ok(averageProvinceData);

            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                         .Returns(importResult);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(It.IsAny<Reading>()))
                                         .Returns(resultConsistentReadingCreation);

            _provinceDataListFactoryMock.Setup(x => x.CreateProvinceDataList(new List<ConsistentReading> { consistentReading }))
                                        .Returns(new List<ProvinceData> { provinceData });

            _averageProvinceDataFactoryMock.Setup(x => x.CreateAverageProvinceData(provinceData))
                                           .Returns(resultAverageProvinceDataCreation);

            // Act
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", outDirectoryTestPath);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            var exportDirectories = Directory.GetDirectories(outDirectoryTestPath);
            var files = Directory.GetFiles(exportDirectories[0]);
            var readingFile = Path.Combine(exportDirectories[0], files[0]);

            Assert.Single(files);
            Assert.Contains(readingFile, files);

            // Teardown
            Directory.Delete(outDirectoryTestPath, true);
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

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                       .Contains("No CSV files found in the 'IN' directory.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

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
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest");

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
            Reading reading = new Reading(123, "Sensor", "ng/m³", 123, "Station", 123, "Province", "City", true, DateTime.Now, null, 123, 123, "Latitude", "Longitude");
            ImportResult importResult = new ImportResult(new List<Reading> { reading }, new List<string>());

            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
             .Returns(importResult);

            Result<ConsistentReading> result = 
                Result<ConsistentReading>.Ko(new List<string> { "ERROR" });

            _consistentReadingFactoryMock.Setup(x => x
                                         .CreateConsistentReading(It.IsAny<Reading>()))
                                         .Returns(result);

            // Act & Assert
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest");

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
            Reading reading = new Reading(123, "Sensor", "ng/m³", 123, "Station", 123, "Province", "City", true, DateTime.Now, null, 123, 123, "Latitude", "Longitude");
            ConsistentReading consistentReading = new ConsistentReading(123, "Sensor", Core.Enum.Unit.ng_m3, 123, "Province", "City", true, 123, 123, "Latitude", "Longitude");
            IList<ConsistentReading> consistentReadings = new List<ConsistentReading> { consistentReading };
            ImportResult importResult = new ImportResult(new List<Reading> { reading }, new List<string>());

            Result<ConsistentReading> resultConsistentReadingCreation = Result<ConsistentReading>.Ok(consistentReading);

            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                         .Returns(importResult);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(It.IsAny<Reading>()))
                                         .Returns(resultConsistentReadingCreation);

            _provinceDataListFactoryMock.Setup(x => x.CreateProvinceDataList(It.IsAny<List<ConsistentReading>>()))
                                        .Returns(new List<ProvinceData>());

            // Act & Assert
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest");

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
            Reading reading = new Reading(123, "Sensor", "ng/m³", 123, "Station", 123, "Province", "City", true, DateTime.Now, null, 123, 123, "Latitude", "Longitude");
            ConsistentReading consistentReading = new ConsistentReading(123, "Sensor", Core.Enum.Unit.ng_m3, 123, "Province", "City", true, 123, 123, "Latitude", "Longitude");
            IList<ConsistentReading> consistentReadings = new List<ConsistentReading> { consistentReading };
            ProvinceData provinceData = new ProvinceData("Province", "Sensor", consistentReadings);
            AverageProvinceData averageProvinceData = new AverageProvinceData("Province", "Sensor", 123, Core.Enum.Unit.ng_m3, 123);
            ImportResult importResult = new ImportResult(new List<Reading> { reading }, new List<string>());

            Result<ConsistentReading> resultConsistentReadingCreation = Result<ConsistentReading>.Ok(consistentReading);
            Result<AverageProvinceData> resultAverageProvinceDataCreation = Result<AverageProvinceData>.Ko(new List<string> { "ERROR" });

            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                                     .Returns(importResult);

            _consistentReadingFactoryMock.Setup(x => x.CreateConsistentReading(It.IsAny<Reading>()))
                                         .Returns(resultConsistentReadingCreation);

            _provinceDataListFactoryMock.Setup(x => x.CreateProvinceDataList(new List<ConsistentReading> { consistentReading }))
                                        .Returns(new List<ProvinceData> { provinceData });

            _averageProvinceDataFactoryMock.Setup(x => x.CreateAverageProvinceData(provinceData))
                                           .Returns(resultAverageProvinceDataCreation);

            // Act & Assert
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\INTest", @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests\OUTTest");

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