using Bogus;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services.Factory;
using SimulationExercise.Tests.ObjectGeneration;

namespace SimulationExercise.Tests.Service
{
    public class AverageProvinceDataFactoryTests
    {
        private readonly AverageProvinceDataFactory _sut;
        private readonly Faker _faker;

        public AverageProvinceDataFactoryTests()
        {
            _sut = new AverageProvinceDataFactory();
            _faker = new Faker();
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldReturnError_WhenProvinceDataIsNull()
        {
            // Arrange
            ProvinceData provinceData = new ProvinceData("Province", "Sensor", new List<ConsistentReading> { });
            
            // Act
            var result = _sut.CreateAverageProvinceData(provinceData);
            
            // Assert
            Assert.False(result.Success);
            Assert.Contains("ProvinceData contains no readings.", result.Errors);
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldReturnNoError_WhenOneProvinceDataInList()
        {
            // Arrange
            ProvinceData provinceData = new ProvinceData("Province", "Sensor", new List<ConsistentReading> { new ConsistentReading(123, "Sensor", Unit.mg_m3, 123, "Province", "City", true, 123, 123, "Latitude", "Longitude") });
            
            // Act
            var result = _sut.CreateAverageProvinceData(provinceData);
            
            // Assert
            Assert.True(result.Success);
        }

        [Theory]
        [MemberData(nameof(ProvinceDataData.GetProvinceData), MemberType = typeof(ProvinceDataData))]
        public void CreateAverageProvinceData_ShouldReturnSuccesfully
                      (ProvinceData provinceData, double averageValue, 
                                  Unit unit, int averageDaysOfMeasure)
        {
            //Act
            var result = _sut.CreateAverageProvinceData(provinceData);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(provinceData.Province, result.Value.Province);
            Assert.Equal(provinceData.SensorTypeName, result.Value.SensorTypeName);
            Assert.Equal(averageValue, result.Value.AverageValue);
            Assert.Equal(unit, result.Value.Unit);
            Assert.Equal(averageDaysOfMeasure, result.Value.AverageDaysOfMeasure);
        }

        [Theory]
        [MemberData(nameof(ProvinceDataData.GetInconsistentProvinceData), MemberType = typeof(ProvinceDataData))]
        public void CreateAverageProvinceData_ShouldReturnErrors_WhenInconsistentUnits(ProvinceData provinceData, List<string> errors)
        {
            // Act
            var result = _sut.CreateAverageProvinceData(provinceData);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(errors, result.Errors);
        }
    }
}
  