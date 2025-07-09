using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services.Factories;

namespace SimulationExercise.Tests.Service
{
    public class ConsistentReadingInsertDTOFactoryTests
    {
        private readonly IConsistentReadingInsertDTOFactory _sut;

        public ConsistentReadingInsertDTOFactoryTests()
        {
            _sut = new ConsistentReadingInsertDTOFactory();
        }

        [Theory]
        [MemberData(nameof(GetConsistentReadings))]
        public void CreateConsistentReadingInsertDTO_ShouldReturnCorrectObject(ConsistentReading consistentReading, ConsistentReadingInsertDTO crInsertDTO)
        {
            // Act & Assert
            var result = _sut.CreateConsistentReadingInsertDTO(consistentReading, 1);
            Assert.Equal(crInsertDTO, result);
        }

        public static IEnumerable<object[]> GetConsistentReadings()
        {
            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.Success)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.Success)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.Success)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.Success)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.Success)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.Success)
            };
        }
    }
}
