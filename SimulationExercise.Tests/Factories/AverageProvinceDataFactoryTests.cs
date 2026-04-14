using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services.Factory;

namespace SimulationExercise.Tests.Factories
{
    public class AverageProvinceDataFactoryTests
    {
        private readonly IAverageProvinceDataFactory _sut;
        private readonly Mock<IValidator<ProvinceData>> _validatorMock;

        public AverageProvinceDataFactoryTests()
        {
            _validatorMock = new Mock<IValidator<ProvinceData>>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_validatorMock.Object);

            _sut = new AverageProvinceDataFactory(_validatorMock.Object);
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldCreateAverageProvinceData()
        {
            // Arrange
            var validationSuccess = new ValidationResult();
            _validatorMock.Setup(v => v.Validate(It.IsAny<ProvinceData>())).Returns(validationSuccess);

            var consistentReadings = new List<ConsistentReading>
            {
                new ConsistentReading(123, "Sensor1", Unit.mg_m3, 2927, "Province1", "City1", true, 9471, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.mg_m3, 3054, "Province1", "City1", true, 8828, 123, 123, "Latitude", "Longitude")    ,
                new ConsistentReading(123, "Sensor1", Unit.mg_m3, 1663, "Province1", "City1", false, 2863, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.mg_m3, 6033, "Province1", "City1", false, 7130, 123, 123, "Latitude", "Longitude"),

                new ConsistentReading(123, "Sensor2", Unit.ng_m3, 5693, "Province2", "City1", true, 1967, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor2", Unit.ng_m3, 3729, "Province2", "City1", true, 2016, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor2", Unit.ng_m3, 9452, "Province2", "City1", false, 5174, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor2", Unit.ng_m3, 9719, "Province2", "City1", false, 7242, 123, 123, "Latitude", "Longitude"),
                
                new ConsistentReading(123, "Sensor3", Unit.µg_m3, 6145, "Province3", "City1", true, 3307, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor3", Unit.µg_m3, 2897, "Province3", "City1", true, 2362, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor3", Unit.µg_m3, 8254, "Province3", "City1", false, 9440, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor3", Unit.µg_m3, 6754, "Province3", "City1", false, 4979, 123, 123, "Latitude", "Longitude"),
            };

            var expectedResults = new List<Result<AverageProvinceData>>
            {
                Result<AverageProvinceData>.Ok(new AverageProvinceData("Province1", "Sensor1", 3419.25, Unit.mg_m3, 7073)),
                Result<AverageProvinceData>.Ok(new AverageProvinceData("Province2", "Sensor2", 7148.25, Unit.ng_m3, 4099)),
                Result<AverageProvinceData>.Ok(new AverageProvinceData("Province3", "Sensor3", 6012.50, Unit.µg_m3, 5022))
            };

            // Act & Assert
            var result = _sut.CreateAverageProvinceData(consistentReadings);
            result.Should().BeEquivalentTo(expectedResults);
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var validationFailureList = new List<ValidationFailure>
            { new ValidationFailure("ERROR", "ERROR") };

            var validationFail = new ValidationResult(validationFailureList);
            _validatorMock.Setup(v => v.Validate(It.IsAny<ProvinceData>())).Returns(validationFail);

            var consistentReadings = new List<ConsistentReading>
            {
                new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123, "Province1", "City1", true, 123, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123, "Province1", "City1", true, 123, 123, 123, "Latitude", "Longitude"),
            };

            // Act & Assert
            var result = _sut.CreateAverageProvinceData(consistentReadings);
            Assert.False(result.First().Success);
            Assert.Single(result.First().Errors!);
            Assert.Equal("ERROR", result.First().Errors?.First());
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldReturnError_WhenNullConsistentReadingList()
        {
            // Act & Assert
            var result = _sut.CreateAverageProvinceData(null!);
            Assert.False(result.First().Success);
            Assert.Single(result.First().Errors!);
            Assert.Equal("Null or empty consistent reading list", result.First().Errors?.First());
        }

        [Fact]
        public void CreateAverageProvinceData_ShouldReturnError_WhenEmptyConsistentReadingList()
        {
            // Act & Assert
            var result = _sut.CreateAverageProvinceData(new List<ConsistentReading>());
            Assert.False(result.First().Success);
            Assert.Single(result.First().Errors!);
            Assert.Equal("Null or empty consistent reading list", result.First().Errors?.First());
        }
    }
}