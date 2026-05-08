using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;
using CSVReader.Core.Enum;
using CSVReader.Services.Factories;

namespace CSVReader.Tests.Factories
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
            // Act
            var result = _sut.CreateConsistentReadingInsertDTO(consistentReading, 1);

            // Assert
            Assert.Equal(crInsertDTO.ReadingId, result.ReadingId);
            Assert.Equal(crInsertDTO.SensorId, result.SensorId);
            Assert.Equal(crInsertDTO.SensorTypeName, result.SensorTypeName);
            Assert.Equal(crInsertDTO.Unit, result.Unit);
            Assert.Equal(crInsertDTO.Value, result.Value);
            Assert.Equal(crInsertDTO.Province, result.Province);
            Assert.Equal(crInsertDTO.City, result.City);
            Assert.Equal(crInsertDTO.IsHistoric, result.IsHistoric);
            Assert.Equal(crInsertDTO.DaysOfMeasure, result.DaysOfMeasure);
            Assert.Equal(crInsertDTO.UtmNord, result.UtmNord);
            Assert.Equal(crInsertDTO.UtmEst, result.UtmEst);
            Assert.Equal(crInsertDTO.Latitude, result.Latitude);
            Assert.Equal(crInsertDTO.Longitude, result.Longitude);
        }

        public static IEnumerable<object[]> GetConsistentReadings()
        {
            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", true)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", false)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", true)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", false)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", true)
            };

            yield return new object[]
            {
                new ConsistentReading(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingInsertDTO(1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", false)
            };
        }
    }
}
