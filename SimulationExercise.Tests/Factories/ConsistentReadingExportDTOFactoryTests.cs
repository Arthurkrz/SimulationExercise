using FluentAssertions;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Tests.Factories
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
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New),

                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude", Status.New),
                new ConsistentReadingGetDTO(1, 1, 1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude", Status.New),
            };

            var expectedCRExportDTOs = new List<ConsistentReadingExportDTO>
            {
                new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.ng_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.mg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),

                new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", true, 1, 1, 1, "Latitude", "Longitude"),
                new ConsistentReadingExportDTO(1, "SensorTypeName", Unit.µg_m3, 1, "Province", "City", false, 1, 1, 1, "Latitude", "Longitude"),
            };

            // Act & Assert
            var result = _sut.CreateExportDTOList(consistentReadingsDTOList);

            result.Should().BeEquivalentTo(expectedCRExportDTOs);
        }
    }
}
