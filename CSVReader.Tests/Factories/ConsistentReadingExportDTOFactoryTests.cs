using FluentAssertions;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Enum;
using CSVReader.Services.Factories;

namespace CSVReader.Tests.Factories
{
    public class ConsistentReadingExportDTOFactoryTests
    {
        private readonly IConsistentReadingExportDTOFactory _sut;

        public ConsistentReadingExportDTOFactoryTests()
        {
            _sut = new ConsistentReadingExportDTOFactory();
        }

        [Fact]
        public void CreateExportDTOList_ShouldCreateExportDTOList()
        {
            // Arrange
            var consistentReadingsDTOList = new List<ConsistentReadingGetDTO>
            {
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", true),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", true),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", false),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", true),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", false)
            };

            var expectedCRExportDTOs = new List<ConsistentReadingExportDTO>
            {
                new ConsistentReadingExportDTO(1, "SensorTypeName", "ng_m3", 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName", "ng_m3", 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                                                                    
                new ConsistentReadingExportDTO(1, "SensorTypeName", "mg_m3", 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName", "mg_m3", 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
                                                                    
                new ConsistentReadingExportDTO(1, "SensorTypeName", "µg_m3", 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName", "µg_m3", 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude")
            };

            // Act
            var result = _sut.CreateExportDTOList(consistentReadingsDTOList);

            // Assert
            result.Should().BeEquivalentTo(expectedCRExportDTOs);
        }
    }
}
