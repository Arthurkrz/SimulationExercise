using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;
using SimulationExercise.Tests.Utilities;

namespace SimulationExercise.Tests.Service
{
    public class OutputFileServiceTests
    {
        private readonly IOutputFileService _sut;
        private readonly Mock<IContextFactory> _contextFactoryMock;
        private readonly Mock<IConsistentReadingRepository> _consistentReadingRepositoryMock;
        private readonly Mock<IOutputFileRepository> _outputFileRepositoryMock;
        private readonly Mock<IConsistentReadingExportDTOFactory> _consistentReadingExportDTOFactoryMock;
        private readonly Mock<ILogger<OutputFileService>> _loggerMock;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;


        public OutputFileServiceTests()
        {
            _contextFactoryMock = new Mock<IContextFactory>();
            _consistentReadingRepositoryMock = new Mock<IConsistentReadingRepository>();
            _outputFileRepositoryMock = new Mock<IOutputFileRepository>();
            _consistentReadingExportDTOFactoryMock = new Mock<IConsistentReadingExportDTOFactory>();
            _loggerMock = new Mock<ILogger<OutputFileService>>();
            _testRepositoryCleanup = new TestRepositoryCleanup();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_contextFactoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingRepositoryMock);
            serviceCollection.AddSingleton(_outputFileRepositoryMock.Object);
            serviceCollection.AddSingleton(_consistentReadingExportDTOFactoryMock);
            serviceCollection.AddSingleton(_loggerMock.Object);
            serviceCollection.AddSingleton(_testRepositoryCleanup);

            _sut = new OutputFileService
            (
                _contextFactoryMock.Object,
                _consistentReadingRepositoryMock.Object,
                _outputFileRepositoryMock.Object,
                _consistentReadingExportDTOFactoryMock.Object,
                _loggerMock.Object
            );

            var contextMock = new Mock<IContext>();
            _contextFactoryMock.Setup(x => x.Create()).Returns(contextMock.Object);
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessConsistentReadings_ShouldProcessConsistentReadings(List<ConsistentReadingGetDTO> crDTOs, List<ConsistentReadingExportDTO> crExportDTOs)
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            _consistentReadingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(crDTOs);

            _consistentReadingExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<ConsistentReadingGetDTO>>()))
                .Returns(crExportDTOs);

            // Act
            _sut.ProcessConsistentReadings();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);

            _consistentReadingRepositoryMock.Verify(x => x.Update(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()), 
                Times.Exactly(6));

            _outputFileRepositoryMock.Verify(x => x.Insert(
                It.IsAny<OutputFileInsertDTO>(), It.IsAny<IContext>()),
                Times.Once);
        }

        [Fact]
        public void ProcessConsistentReadings_ShouldLogError_WhenNoNewObjectsFound()
        {
            // Arrange
            _consistentReadingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(new List<ConsistentReadingGetDTO>());

            // Act & Assert
            _sut.ProcessConsistentReadings();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("No new Consistent Readings have been found!")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessConsistentReadings_ShouldLogError_WhenFailToInsert(List<ConsistentReadingGetDTO> crDTOs, List<ConsistentReadingExportDTO> crExportDTOs)
        {
            // Arrange
            _consistentReadingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(crDTOs);

            _consistentReadingExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<ConsistentReadingGetDTO>>()))
                .Returns(crExportDTOs);

            _outputFileRepositoryMock.Setup(x => x.Insert(
                It.IsAny<OutputFileInsertDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Insert failed"));

            // Act & Assert
            _sut.ProcessConsistentReadings();

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

        [Theory]
        [MemberData(nameof(GetValidObjects))]
        public void ProcessConsistentReadings_ShouldLogError_WhenFailToUpdate(List<ConsistentReadingGetDTO> crDTOs, List<ConsistentReadingExportDTO> crExportDTOs)
        {
            // Arrange
            _consistentReadingRepositoryMock.Setup(x => x.GetByStatus(
                It.IsAny<Status>(), It.IsAny<IContext>()))
                .Returns(crDTOs);

            _consistentReadingExportDTOFactoryMock.Setup(x => x.CreateExportDTOList(
                It.IsAny<IList<ConsistentReadingGetDTO>>()))
                .Returns(crExportDTOs);

            _consistentReadingRepositoryMock.Setup(x => x.Update(
                It.IsAny<ConsistentReadingUpdateDTO>(), It.IsAny<IContext>()))
                .Throws(new Exception("Update failed"));

            // Act & Assert
            _sut.ProcessConsistentReadings();

            _loggerMock.Verify(
                x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                                                        .Contains("Unexpected exception was thrown: Update failed")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        public static IEnumerable<object[]> GetValidObjects()
        {
            yield return new object[]
            {
                new List<ConsistentReadingGetDTO>
                {
                    new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                    new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New),

                    new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                    new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New),

                    new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                    new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New),
                },

                new List<ConsistentReadingExportDTO>
                {
                    new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                    new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                    new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                    new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                    new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                    new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                }
            };
        }
    }
}