using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Services;
using SimulationExercise.Tests.Repository;
using System.Text;

namespace SimulationExercise.Tests.Service
{
    public class InputFileServiceTests
    {
        private readonly IInputFileService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IInputFileRepository> _inputFileRepositoryMock;
        private readonly Mock<ILogger<InputFileService>> _loggerMock;
        private readonly ITestRepositoryCleanup _testRepositoryCleanup;

        private readonly string _basePath;
        private readonly string _inDirectoryPath;

        public InputFileServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _inputFileRepositoryMock = new Mock<IInputFileRepository>();
            _loggerMock = new Mock<ILogger<InputFileService>>();
            _testRepositoryCleanup = new TestRepositoryCleanup();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_inputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_loggerMock.Object);

            _sut = new InputFileService
            (
                _contextFactoryMock.Object,
                _inputFileRepositoryMock.Object,
                _loggerMock.Object
            );

            _basePath = Path.Combine(Path.GetTempPath(), "SimulationExerciseTests");
            _inDirectoryPath = Path.Combine(_basePath, "IN");

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Fact]
        public void ProcessFiles_ShouldProcessFiles()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            DirectoryCleanup();
            InputFileGenerator(2);

            // Act
            _sut.ProcessFiles(_inDirectoryPath);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _inputFileRepositoryMock.Verify(x => x.Insert(
                It.IsAny<InputFileInsertDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(2));
        }

        [Fact]
        public void ProcessFiles_ShouldLogError_WhenEmptyFile()
        {
            // Arrange
            DirectoryCleanup();

            var bytesInputText = Encoding.UTF8.GetBytes("");
            var inputStream = new MemoryStream(bytesInputText);

            string inFilePath = Path.Combine(_inDirectoryPath, $"CSVTest.csv");

            using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
            {
                inputStream.Position = 0;
                inputStream.CopyTo(fileStream);
            }

            // Act & Assert
            _sut.ProcessFiles(_inDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains(LogMessages.EMPTYFILE)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFiles_ShouldLogError_WhenFailToInsert()
        {
            // Arrange
            DirectoryCleanup();
            InputFileGenerator(1);

            _inputFileRepositoryMock.Setup(x => x.Insert(
                It.IsAny<InputFileInsertDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Insert failed"));

            // Act & Assert
            _sut.ProcessFiles(_inDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Insert failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void LocateFiles_ShouldLogError_WhenNoFilesFound()
        {
            // Arrange
            DirectoryCleanup();

            // Act & Assert
            _sut.ProcessFiles(_inDirectoryPath);

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Value cannot be null. " +
                                                                  "(Parameter 'No CSV files found in the 'IN' directory!')")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFiles_ShouldCreateDirectory_WhenINDirectoryNotFound()
        {
            // Arrange
            if (Directory.Exists(_inDirectoryPath)) Directory.Delete(_inDirectoryPath, true);

            // Act
            _sut.ProcessFiles(_inDirectoryPath);

            // Assert
            var directories = Directory.GetDirectories(_basePath);
            Assert.True(directories.Any(x => x.Contains(_inDirectoryPath)));
        }

        private void DirectoryCleanup()
        {
            if (Directory.Exists(_inDirectoryPath)) Directory.Delete(_inDirectoryPath, true);
            Directory.CreateDirectory(_inDirectoryPath);
        }

        private void InputFileGenerator(int numberOfInputFilesToBeInserted, bool containsWrongReadings = false)
        {
            string inputText = null;

            if (containsWrongReadings) inputText = @"IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location
12453,Arsenico,ng/mÂ³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
5712,Ozono,Âµg/mÂ³,ERROR,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
20488,Particelle sospese PM2.5,Âµg/mÂ³,564,Erba v. Battisti,ERROR,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)";

            inputText = @"IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location
12453,Arsenico,ng/mÂ³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
5712,Ozono,Âµg/mÂ³,264,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
20488,Particelle sospese PM2.5,Âµg/mÂ³,564,Erba v. Battisti,538,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)";

            var bytesInputText = Encoding.UTF8.GetBytes(inputText);
            var inputStream = new MemoryStream(bytesInputText);

            for (int objectNumber = 0; objectNumber < numberOfInputFilesToBeInserted; objectNumber++)
            {
                string inFilePath = Path.Combine(_inDirectoryPath, $"CSVTest{objectNumber}.csv");

                using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
                {
                    inputStream.Position = 0;
                    inputStream.CopyTo(fileStream);
                }
            }
        }
    }
}