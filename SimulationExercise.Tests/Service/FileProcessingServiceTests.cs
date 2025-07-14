using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;
using SimulationExercise.Services;
using System.Text;

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

        private readonly string _basePath;
        private readonly string _inDirectoryPath;
        private readonly string _outDirectoryPath;

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

            _sut = new FileProcessingService
            (
                _readingImportServiceMock.Object,
                _consistentReadingFactoryMock.Object,
                _provinceDataListFactoryMock.Object,
                _averageProvinceDataFactoryMock.Object,
                _averageProvinceDataExportServiceMock.Object,
                _loggerMock.Object
            );

            _basePath = Path.Combine(Path.GetTempPath(), "SimulationExerciseTests");
            _inDirectoryPath = Path.Combine(_basePath, "INTest");
            _outDirectoryPath = Path.Combine(_basePath, "OUTTest");
        }

        [Fact]
        public void ProcessFile_ShouldProcessFile()
        {
            // Arrange
            DirectoryCleanup();
            InputFileGenerator();

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
            // Act & Assert
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            var exportDirectories = Directory.GetDirectories(_outDirectoryPath);
            Assert.Single(exportDirectories);

            var outFiles = Directory.GetFiles(exportDirectories[0]);
            Assert.Single(outFiles);

            var readingFile = Path.Combine(exportDirectories[0], "AverageProvinceData.csv");
            Assert.Contains(readingFile, outFiles);
        }

        [Fact]
        public void ProcessFile_ShouldCreateDirectory_WhenNoInDirectoryFound()
        {
            // Arrange
            DirectoryCleanup();
            string nonExistentInDirectoryTestPath = Path.Combine(Environment.CurrentDirectory, 
                                                                "NonExistentINDirectoryPath");
            // Act & Assert
            _sut.ProcessFile(nonExistentInDirectoryTestPath, "OutputPath");
            Assert.True(Directory.Exists(nonExistentInDirectoryTestPath));
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoCSVFilesFound()
        {
            // Arrange
            DirectoryCleanup();

            // Act & Assert
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No CSV files found in the 'IN' directory!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoReadingsImported()
        {
            // Arrange
            DirectoryCleanup();
            InputFileGenerator();

            _readingImportServiceMock.Setup(x => x.Import(It.IsAny<Stream>()))
                .Returns(new ImportResult(new List<Reading>(), new List<string> { "ERROR" }));

            // Act & Assert
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

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
                                                        .Contains("No readings have been imported from file {0}!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoConsistentReadingsCreated()
        {
            // Arrange
            DirectoryCleanup();
            InputFileGenerator();

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
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

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
                                                        .Contains("No ConsistentReadings have been created!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoProvinceDataCreated()
        {
            // Arrange
            DirectoryCleanup();
            InputFileGenerator();

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
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No ProvinceDatas have been created!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFile_ShouldReturnError_WhenNoAverageProvinceDataCreated()
        {
            // Arrange
            DirectoryCleanup();
            InputFileGenerator();

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
            _sut.ProcessFile(_inDirectoryPath, _outDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No AverageProvinceDatas have been created!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        private void DirectoryCleanup()
        {
            if (Directory.Exists(_inDirectoryPath)) Directory.Delete(_inDirectoryPath, true);
            Directory.CreateDirectory(_inDirectoryPath);

            if (Directory.Exists(_outDirectoryPath)) Directory.Delete(_outDirectoryPath, true);
            Directory.CreateDirectory(_outDirectoryPath);
        }

        private void InputFileGenerator()
        {
            var inputText = @"IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location
12453,Arsenico,ng/mÂ³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
5712,Ozono,Âµg/mÂ³,ERROR,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
20488,Particelle sospese PM2.5,Âµg/mÂ³,564,Erba v. Battisti,ERROR,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)";

            var bytesInputText = Encoding.UTF8.GetBytes(inputText);
            var inputStream = new MemoryStream(bytesInputText);

            string inFilePath = Path.Combine(_inDirectoryPath, "CSVTest.csv");
            using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
            {
                inputStream.Position = 0;
                inputStream.CopyTo(fileStream);
            }
        }
    }
}