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
using System.Text;

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
        private readonly Mock<IContext> _contextMock;
        private readonly Mock<ILogger<ReadingService>> _loggerMock;
        private readonly ITestRepositoryCleanup _testRepositoryCleanup;

        public ReadingServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _inputFileRepositoryMock = new Mock<IInputFileRepository>();
            _readingImportServiceMock = new Mock<IReadingImportService>();
            _readingRepositoryMock = new Mock<IReadingRepository>();
            _readingInsertDTOFactoryMock = new Mock<IReadingInsertDTOFactory>();
            _contextMock = new Mock<IContext>();
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

            _contextFactoryMock.Setup(x => x.Create()).Returns(_contextMock.Object);
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessInputFiles_ShouldProcessInputFiles(List<InputFileGetDTO> inputFileDTOList, ImportResult importResult, List<ReadingInsertDTO> insertDTOs)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(inputFileDTOList);

            _readingImportServiceMock.Setup(x => x.Import(
                It.IsAny<Stream>())).Returns(importResult);

            _readingInsertDTOFactoryMock.Setup(x => x.CreateReadingInsertDTOList(
                It.IsAny<IList<Reading>>(), It.IsAny<long>()))
                .Returns(insertDTOs);

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
                Times.Exactly(2));

            _readingRepositoryMock.Verify(x => x.Insert(
                It.IsAny<ReadingInsertDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(6));

            _contextMock.Verify(x => x.Commit(), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(GetInvalidObjects))]
        public void ProcessInputFiles_ShouldLogErrors_WhenInvalidRecords(List<InputFileGetDTO> inputFileDTOList, ImportResult importResultWithErrors)
        {
            // Arrange
            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(inputFileDTOList);

            _readingImportServiceMock.Setup(x => x.Import(
                It.IsAny<Stream>())).Returns(importResultWithErrors);

            // Act
            _sut.ProcessInputFiles();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Error Converting 'ERROR' to type: 'Int64'. ")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(4));

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Error Converting 'ERROR' to type: 'Int32'. ")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(2));

            _inputFileRepositoryMock.Verify(x => x.Update(
                It.IsAny<InputFileUpdateDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(2));

            _contextMock.Verify(x => x.Commit(), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessInputFiles_ShouldLogError_IfNoNewObjectsFound(List<InputFileGetDTO> inputFileDTOs, ImportResult importResult, List<ReadingInsertDTO> insertDTOs)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<InputFileGetDTO>());

            _readingImportServiceMock.Setup(x => x.Import(
                It.IsAny<Stream>())).Returns(importResult);

            _readingInsertDTOFactoryMock.Setup(x => x.CreateReadingInsertDTOList(
                It.IsAny<IList<Reading>>(), It.IsAny<long>()))
                .Returns(insertDTOs);

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

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessInputFiles_ShouldLogError_WhenFailToInsert(List<InputFileGetDTO> inputFileDTOList, ImportResult importResult, List<ReadingInsertDTO> insertDTOs)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(inputFileDTOList);

            _readingImportServiceMock.Setup(x => x.Import(
                It.IsAny<Stream>())).Returns(importResult);

            _readingInsertDTOFactoryMock.Setup(x => x.CreateReadingInsertDTOList(
                It.IsAny<IList<Reading>>(), It.IsAny<long>()))
                .Returns(insertDTOs);

            _readingRepositoryMock.Setup(x => x.Insert(
                It.IsAny<ReadingInsertDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Insert failed"));

            // Act & Assert
            _sut.ProcessInputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Insert failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(GetInvalidObjects))]
        public void ProcessInputFiles_ShouldLogError_WhenFailToUpdate(List<InputFileGetDTO> inputFileDTOList, ImportResult importResultWithErrors)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _inputFileRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(inputFileDTOList);

            _readingImportServiceMock.Setup(x => x.Import(
                It.IsAny<Stream>())).Returns(importResultWithErrors);

            _inputFileRepositoryMock.Setup(x => x.Update(
                It.IsAny<InputFileUpdateDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Update failed"));

            // Act & Assert
            _sut.ProcessInputFiles();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Update failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(2));
        }

        public static IEnumerable<object[]> GetValidObjects()
        {
            var records = @"IdSensore, NomeTipoSensore, UnitaMisura, Idstazione, NomeStazione, Quota, Provincia, Comune, Storico, DataStart, DataStop, Utm_Nord, UTM_Est, lat, lng, Location
                            12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
                            5712,Ozono,µg/m³,510,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
                            20488,Particelle sospese PM2.5,µg/m³,564,Erba v.Battisti,279,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)";

            var bytes = Encoding.UTF8.GetBytes(records);

            var readingList = new List<Reading>
            {
                new Reading(12691, "Arsenico", "ng/m³", 560, "Varese v.Copelli", 383, "VA", "Varese", false, new DateTime(2008, 04, 01), null, 5073728, 486035, "41.8169745", "8.8204911"),
                new Reading(5721, "Ozono", "µg/m³", 510, "Inzago v.le Gramsci", 138, "MI", "Inzago", true, new DateTime(2001, 02, 24), new DateTime(2018, 01, 01), 5043030, 538012, "45.53976956", "9.48689669"),
                new Reading(20488, "Particelle sospese PM2.5", "µg/m³", 564, "Erba v.Battisti", 279, "CO", "Erba", false, new DateTime(2020, 10, 22), null, 5072803, 517232, "45.8085738", "9.2217792")
            };

            yield return new object[]
            {
                new List<InputFileGetDTO>
                {
                    new InputFileGetDTO(1, "TestFile", bytes, ",csv", Status.New),
                    new InputFileGetDTO(1, "TestFile", bytes, ",csv", Status.New)
                },

                new ImportResult(readingList, new List<string>()),

                new List<ReadingInsertDTO>
                {
                    new ReadingInsertDTO(1, 12691, "Arsenico", "ng/m³", 560, "Varese v.Copelli", 383, "VA", "Varese", false, new DateTime(2008, 04, 01), null, 5073728, 486035, "41.8169745", "8.8204911", Status.New),
                    new ReadingInsertDTO(1, 5721, "Ozono", "µg/m³", 510, "Inzago v.le Gramsci", 138, "MI", "Inzago", true, new DateTime(2001, 02, 24), new DateTime(2018, 01, 01), 5043030, 538012, "45.53976956", "9.48689669", Status.New),
                    new ReadingInsertDTO(1, 20488, "Particelle sospese PM2.5", "µg/m³", 564, "Erba v.Battisti", 279, "CO", "Erba", false, new DateTime(2020, 10, 22), null, 5072803, 517232, "45.8085738", "9.2217792", Status.New)
                }
            };
        }

        public static IEnumerable<object[]> GetInvalidObjects()
        {
            var invalidRecords = @"ERROR, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
                                   12691, Arsenico, ng/m³,ERROR, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
                                   12691, Arsenico, ng/m³,560, Varese v.Copelli,ERROR, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)";

            var bytes = Encoding.UTF8.GetBytes(invalidRecords);

            var errorList = new List<string>
            {
                "Error Converting 'ERROR' to type: 'Int64'. ",
                "Error Converting 'ERROR' to type: 'Int64'. ",
                "Error Converting 'ERROR' to type: 'Int32'. "
            };

            yield return new object[]
            {
                new List<InputFileGetDTO>
                {
                    new InputFileGetDTO(1, "TestFile", bytes, ",csv", Status.New),
                    new InputFileGetDTO(1, "TestFile", bytes, ",csv", Status.New)
                },

                new ImportResult(new List<Reading>(), errorList)
            };
        }
    }
}
