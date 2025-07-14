using FluentAssertions;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services.Factories;

namespace SimulationExercise.Tests.Service
{
    public class ReadingInsertDTOFactoryTests
    {
        private readonly IReadingInsertDTOFactory _sut;

        public ReadingInsertDTOFactoryTests()
        {
            _sut = new ReadingInsertDTOFactory();
        }

        [Fact]
        public void CreateReadingInsertDTOList_ShouldReturnCorrectList()
        {
            // Arrange
            var startDate = DateTime.Now.AddYears(-1);

            var readings = new List<Reading>
            {
                new Reading(1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, null, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, null, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, null, 1, 1, "Latitude", "Longitude")
            };

            var expectedInsertDTOList = new List<ReadingInsertDTO>
            {
                new ReadingInsertDTO(1, 1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude", Status.New),
                new ReadingInsertDTO(1, 1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude", Status.New),
                new ReadingInsertDTO(1, 1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude", Status.New),
                new ReadingInsertDTO(1, 1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude", Status.New),
                new ReadingInsertDTO(1, 1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude", Status.New),
                new ReadingInsertDTO(1, 1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1).Date, DateTime.Now.Date, 1, 1, "Latitude", "Longitude", Status.New)
            };

            // Act & Assert
            var result = _sut.CreateReadingInsertDTOList(readings, 1);
            expectedInsertDTOList.Should().BeEquivalentTo(result);
        }
    }
}
