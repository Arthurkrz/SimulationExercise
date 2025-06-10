using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services.Factory;

namespace SimulationExercise.Tests.Service
{
    public class ProvinceDataListFactoryTests
    {
        private readonly ProvinceDataListFactory _sut;

        public ProvinceDataListFactoryTests()
        {
            _sut = new ProvinceDataListFactory();
        }

        [Theory]
        [MemberData(nameof(GetConsistentReadings))]
        public void CreateProvinceDataList_ShouldGroupSuccesfully(
                               List<ConsistentReading> consistentReadings,
                               Func<ProvinceData, bool> filterLogic,
                               int expectedFilteredResultReadingsCount, 
                               Func<ConsistentReading, bool> assertionLogic)
        {
            // Act
            var result = _sut.CreateProvinceDataList(consistentReadings);
            var filteredResult = result.SingleOrDefault(filterLogic);

            // Assert
            Assert.NotNull(filteredResult);
            Assert.Equal(expectedFilteredResultReadingsCount,
                         filteredResult!.ConsistentReadings.Count);
            Assert.True(filteredResult!.ConsistentReadings
                                       .All(assertionLogic));
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnEmptyList_WhenEmptyConsistentReadingList()
        {
            // Arrange
            var emptyConsistentReadings = new List<ConsistentReading>();

            // Act
            var result = _sut.CreateProvinceDataList(emptyConsistentReadings);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnEmptyList_WhenNullConsistentReadingList()
        {
            // Arrange
            List<ConsistentReading> nullConsistentReadings;

            // Act
            var result = _sut.CreateProvinceDataList(nullConsistentReadings = null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnListSuccesfully()
        {
            // Arrange
            var consistentReadings = new List<ConsistentReading>
            {
                new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.µg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor1", Unit.µg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor2", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor2", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province3", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province2", "City1", true, 123, 123, "Latitude", "Longitude"),
                new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province3", "City1", true, 123, 123, "Latitude", "Longitude"),
            };

            // Act
            var result = _sut.CreateProvinceDataList(consistentReadings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);

            var filteredResult = result
                .Single(x => x.SensorTypeName == "Sensor1" &&
                             x.Province == "Province1" &&
                             x.ConsistentReadings.Any
                            (x => x.Unit == Unit.ng_m3));

            Assert.NotNull(filteredResult);
            Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            Assert.True(filteredResult.ConsistentReadings.All(
                x => x.Unit == Unit.ng_m3 &&
                     x.SensorTypeName == "Sensor1" &&
                     x.Province == "Province1"));

            filteredResult = result
                .Single(x => x.SensorTypeName == "Sensor1" &&
                             x.Province == "Province1" &&
                             x.ConsistentReadings.Any
                            (x => x.Unit == Unit.mg_m3));

            Assert.NotNull(filteredResult);
            Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            Assert.True(filteredResult.ConsistentReadings.All(
                x => x.Unit == Unit.mg_m3 &&
                     x.SensorTypeName == "Sensor1" &&
                     x.Province == "Province1"));

            filteredResult = result
                .Single(x => x.SensorTypeName == "Sensor1" &&
                             x.Province == "Province1" &&
                             x.ConsistentReadings.Any
                            (x => x.Unit == Unit.µg_m3));

            Assert.NotNull(filteredResult);
            Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            Assert.True(filteredResult.ConsistentReadings.All(
                x => x.Unit == Unit.µg_m3 &&
                     x.SensorTypeName == "Sensor1" &&
                     x.Province == "Province1"));

            filteredResult = result
                .Single(x => x.SensorTypeName == "Sensor2" &&
                             x.Province == "Province1" &&
                             x.ConsistentReadings.Any
                            (x => x.Unit == Unit.mg_m3));

            Assert.NotNull(filteredResult);
            Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            Assert.True(filteredResult.ConsistentReadings.All(
                x => x.Unit == Unit.mg_m3 &&
                     x.SensorTypeName == "Sensor2" &&
                     x.Province == "Province1"));

            filteredResult = result
                .Single(x => x.SensorTypeName == "Sensor3" &&
                             x.Province == "Province3" &&
                             x.ConsistentReadings.Any
                            (x => x.Unit == Unit.ng_m3));

            Assert.NotNull(filteredResult);
            Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            Assert.True(filteredResult.ConsistentReadings.All(
                x => x.Unit == Unit.ng_m3 &&
                     x.SensorTypeName == "Sensor3" &&
                     x.Province == "Province3"));

            filteredResult = result
                .Single(x => x.SensorTypeName == "Sensor3" &&
                             x.Province == "Province2" &&
                             x.ConsistentReadings.Any
                            (x => x.Unit == Unit.ng_m3));

            Assert.NotNull(filteredResult);
            Assert.Equal(1, filteredResult.ConsistentReadings.Count);
            Assert.True(filteredResult.ConsistentReadings.All(
                x => x.Unit == Unit.ng_m3 &&
                     x.SensorTypeName == "Sensor3" &&
                     x.Province == "Province2"));
        }

        public static IEnumerable<object[]> GetConsistentReadings()
        {
            yield return new object[]
            {
                new List<ConsistentReading>
                {
                    new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                    new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                },

                new Func<ProvinceData, bool>(x =>
                                             x.SensorTypeName == "Sensor1" &&
                                             x.Province == "Province1" &&
                                             x.ConsistentReadings.Any
                                            (x => x.Unit == Unit.ng_m3)), 2,

                new Func<ConsistentReading, bool>(x =>
                                                  x.Unit == Unit.ng_m3 &&
                                                  x.SensorTypeName == "Sensor1" &&
                                                  x.Province == "Province1")
            };

            yield return new object[]
            {
                new List<ConsistentReading>
                {
                    new ConsistentReading(123, "Sensor1", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                    new ConsistentReading(123, "Sensor1", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                },

                new Func<ProvinceData, bool>(x =>
                                             x.SensorTypeName == "Sensor1" &&
                                             x.Province == "Province1" &&
                                             x.ConsistentReadings.Any
                                            (x => x.Unit == Unit.mg_m3)), 2,

                new Func<ConsistentReading, bool>(x =>
                                                  x.Unit == Unit.mg_m3 &&
                                                  x.SensorTypeName == "Sensor1" &&
                                                  x.Province == "Province1")
            };

            yield return new object[]
            {
                new List<ConsistentReading>
                {
                    new ConsistentReading(123, "Sensor1", Unit.µg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                    new ConsistentReading(123, "Sensor1", Unit.µg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                },

                new Func<ProvinceData, bool>(x =>
                                             x.SensorTypeName == "Sensor1" &&
                                             x.Province == "Province1" &&
                                             x.ConsistentReadings.Any
                                            (x => x.Unit == Unit.µg_m3)), 2,

                new Func<ConsistentReading, bool>(x =>
                                                  x.Unit == Unit.µg_m3 &&
                                                  x.SensorTypeName == "Sensor1" &&
                                                  x.Province == "Province1")
            };

            yield return new object[]
            {
                new List<ConsistentReading>
                {
                    new ConsistentReading(123, "Sensor2", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                    new ConsistentReading(123, "Sensor2", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
                },

                new Func<ProvinceData, bool>(x =>
                                             x.SensorTypeName == "Sensor2" &&
                                             x.Province == "Province1" &&
                                             x.ConsistentReadings.Any
                                            (x => x.Unit == Unit.mg_m3)), 2,

                new Func<ConsistentReading, bool>(x =>
                                                  x.Unit == Unit.mg_m3 &&
                                                  x.SensorTypeName == "Sensor2" &&
                                                  x.Province == "Province1")

            };

            yield return new object[]
            {
                new List<ConsistentReading>
                {
                    new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province2", "City1", true, 123, 123, "Latitude", "Longitude"),
                },

                new Func<ProvinceData, bool>(x =>
                                             x.SensorTypeName == "Sensor3" &&
                                             x.Province == "Province2" &&
                                             x.ConsistentReadings.Any
                                            (x => x.Unit == Unit.ng_m3)), 1,

                new Func<ConsistentReading, bool>(x =>
                                                  x.Unit == Unit.ng_m3 &&
                                                  x.SensorTypeName == "Sensor3" &&
                                                  x.Province == "Province2")

            };

            yield return new object[]
            {
                new List<ConsistentReading>
                {
                    new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province3", "City1", true, 123, 123, "Latitude", "Longitude"),
                    new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province3", "City1", true, 123, 123, "Latitude", "Longitude"),
                },

                new Func<ProvinceData, bool>(x =>
                                             x.SensorTypeName == "Sensor3" &&
                                             x.Province == "Province3" &&
                                             x.ConsistentReadings.Any
                                            (x => x.Unit == Unit.ng_m3)), 2,

                new Func<ConsistentReading, bool>(x => 
                                                  x.Unit == Unit.ng_m3 &&
                                                  x.SensorTypeName == "Sensor3" &&
                                                  x.Province == "Province3")

            };
        }
    }
}