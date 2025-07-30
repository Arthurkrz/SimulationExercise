using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Tests.ObjectGeneration;

namespace SimulationExercise.Tests.Factories
{
    public class AverageProvinceDataFactoryTests
    {
        [Fact]
        public void CreateAverageProvinceData_ShouldReturnError_WhenProvinceDataIsNull()
        {
            throw new NotImplementedException();
            //ProvinceData provinceData = new ProvinceData("Province", "Sensor", new List<ConsistentReading> { });            
            //var result = _sut.CreateAverageProvinceData(provinceData);            
            //Assert.False(result.Success);
            //Assert.Contains("ProvinceData contains no readings.", result.Errors!);
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldReturnNoError_WhenOneProvinceDataInList()
        {
            throw new NotImplementedException();
            //ProvinceData provinceData = new ProvinceData("Province", "Sensor", new List<ConsistentReading> { new ConsistentReading(123, "Sensor", Unit.mg_m3, 123, "Province", "City", true, 123, 123, "Latitude", "Longitude") });
            //var result = _sut.CreateAverageProvinceData(provinceData);
            //Assert.True(result.Success);
        }

        [Theory]
        [MemberData(nameof(ProvinceDataData.GetProvinceData), MemberType = typeof(ProvinceDataData))]
        public void CreateAverageProvinceData_ShouldReturnSuccesfully
                      (ProvinceData provinceData, double averageValue, 
                                  Unit unit, int averageDaysOfMeasure)
        {
            throw new NotImplementedException();
            //var result = _sut.CreateAverageProvinceData(provinceData);
            //Assert.True(result.Success);
            //Assert.Equal(provinceData.Province, result.Value?.Province);
            //Assert.Equal(provinceData.SensorTypeName, result.Value?.SensorTypeName);
            //Assert.Equal(averageValue, result.Value?.AverageValue);
            //Assert.Equal(unit, result.Value?.Unit);
            //Assert.Equal(averageDaysOfMeasure, result.Value?.AverageDaysOfMeasure);
        }

        [Theory]
        [MemberData(nameof(ProvinceDataData.GetInconsistentProvinceData), MemberType = typeof(ProvinceDataData))]
        public void CreateAverageProvinceData_ShouldReturnErrors_WhenInconsistentUnits(ProvinceData provinceData, List<string> errors)
        {
            throw new NotImplementedException();
            //var result = _sut.CreateAverageProvinceData(provinceData);
            //Assert.False(result.Success);
            //Assert.Equal(errors, result.Errors);
        }
    }
}
  