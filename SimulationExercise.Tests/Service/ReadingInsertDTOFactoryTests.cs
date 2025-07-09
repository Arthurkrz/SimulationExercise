using FluentAssertions;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;
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
                new Reading(1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1), DateTime.Now, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1), DateTime.Now, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1), DateTime.Now, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "ng/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1), null, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "mg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1), null, 1, 1, "Latitude", "Longitude"),
                new Reading(1, "SensorTypeName", "µg/m³", 1, "StationName", 1, "Province", "City", true, DateTime.Now.AddYears(-1), null, 1, 1, "Latitude", "Longitude")
            };

            // Act & Assert
            var result = _sut.CreateReadingInsertDTOList(readings, 1);
            readings.Should().BeEquivalentTo(result);
        }
    }
}
