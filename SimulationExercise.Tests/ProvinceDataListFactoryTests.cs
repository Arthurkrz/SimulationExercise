using SimulationExercise.Core;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;

namespace SimulationExercise.Tests
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
                               int expectedFilteredResultReadingsCount)
        {
            // Act
            var result = _sut.CreateProvinceDataList(consistentReadings);
            var filteredResult = result.SingleOrDefault(filterLogic);

            // Assert
            Assert.NotNull(filteredResult);
            Assert.Equal(expectedFilteredResultReadingsCount,
                         filteredResult!.ConsistentReadings.Count);
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnEmptyList_WhenNoConsistentReadingsOrNull()
        {
            // Arrange
            var emptyConsistentReadings = new List<ConsistentReading>();
            List<ConsistentReading> nullConsistentReadings;

            // Act
            var result = _sut.CreateProvinceDataList(emptyConsistentReadings);
            var result2 = _sut.CreateProvinceDataList(nullConsistentReadings = null);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result2);

            Assert.Empty(result);
            Assert.Empty(result2);
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

            //filteredResult = result
            //    .Where(x => x.SensorTypeName == "Sensor1" &&
            //                x.Province == "Province1" &&
            //                x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.mg_m3)).ToList();

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.Count);

            //filteredResult = result
            //    .Where(x => x.SensorTypeName == "Sensor1" &&
            //                x.Province == "Province1" &&
            //                x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.µg_m3)).ToList();

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.Count);

            //filteredResult = result
            //    .Where(x => x.SensorTypeName == "Sensor2" &&
            //                x.Province == "Province1" &&
            //                x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.mg_m3)).ToList();
            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.Count);

            //filteredResult = result
            //    .Where(x => x.SensorTypeName == "Sensor3" &&
            //                x.Province == "Province3" &&
            //                x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.ng_m3)).ToList();

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.Count);

            //filteredResult = result
            //    .Where(x => x.SensorTypeName == "Sensor3" &&
            //                x.Province == "Province2" &&
            //                x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.ng_m3)).ToList();

            //Assert.NotNull(filteredResult);
            //Assert.Equal(1, filteredResult.Count);
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
                                            (x => x.Unit == Unit.ng_m3)), 2
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
                                            (x => x.Unit == Unit.mg_m3)), 2
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
                                            (x => x.Unit == Unit.µg_m3)), 2
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
                                            (x => x.Unit == Unit.mg_m3)), 2

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
                                            (x => x.Unit == Unit.ng_m3)), 1

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
                                            (x => x.Unit == Unit.ng_m3)), 2

            };
        }
    }
}