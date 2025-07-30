using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Tests.Factories
{
    public class ProvinceDataListFactoryTests
    {
        [Theory]
        [MemberData(nameof(GetConsistentReadings))]
        public void CreateProvinceDataList_ShouldGroupSuccesfully(List<ConsistentReading> consistentReadings, Func<ProvinceData, bool> filterLogic, int expectedFilteredResultReadingsCount, Func<ConsistentReading, bool> assertionLogic)
        {
            throw new NotImplementedException();
            //var result = _sut.CreateProvinceDataList(consistentReadings);
            //var filteredResult = result.SingleOrDefault(filterLogic);
            //Assert.NotNull(filteredResult);
            //Assert.Equal(expectedFilteredResultReadingsCount, filteredResult!.ConsistentReadings.Count);
            //Assert.True(filteredResult!.ConsistentReadings.All(assertionLogic));
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnEmptyList_WhenEmptyConsistentReadingList()
        {
            throw new NotImplementedException();
            //var emptyConsistentReadings = new List<ConsistentReading>();
            //var result = _sut.CreateProvinceDataList(emptyConsistentReadings);
            //Assert.NotNull(result);
            //Assert.Empty(result);
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnEmptyList_WhenNullConsistentReadingList()
        {
            throw new NotImplementedException();
            //List<ConsistentReading> nullConsistentReadings = new();
            //var result = _sut.CreateProvinceDataList(nullConsistentReadings);
            //Assert.NotNull(result);
            //Assert.Empty(result);
        }

        [Fact]
        public void CreateProvinceDataList_ShouldReturnListSuccesfully()
        {
            throw new NotImplementedException();
            //var consistentReadings = new List<ConsistentReading>
            //{
            //    new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor1", Unit.ng_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor1", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor1", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor1", Unit.µg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor1", Unit.µg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor2", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor2", Unit.mg_m3, 123,"Province1", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province3", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province2", "City1", true, 123, 123, "Latitude", "Longitude"),
            //    new ConsistentReading(123, "Sensor3", Unit.ng_m3, 123,"Province3", "City1", true, 123, 123, "Latitude", "Longitude"),
            //};

            //var result = _sut.CreateProvinceDataList(consistentReadings);

            //Assert.NotNull(result);
            //Assert.Equal(6, result.Count);

            //var filteredResult = result
            //    .Single(x => x.SensorTypeName == "Sensor1" &&
            //                 x.Province == "Province1" &&
            //                 x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.ng_m3));

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            //Assert.True(filteredResult.ConsistentReadings.All(
            //    x => x.Unit == Unit.ng_m3 &&
            //         x.SensorTypeName == "Sensor1" &&
            //         x.Province == "Province1"));

            //filteredResult = result
            //    .Single(x => x.SensorTypeName == "Sensor1" &&
            //                 x.Province == "Province1" &&
            //                 x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.mg_m3));

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            //Assert.True(filteredResult.ConsistentReadings.All(
            //    x => x.Unit == Unit.mg_m3 &&
            //         x.SensorTypeName == "Sensor1" &&
            //         x.Province == "Province1"));

            //filteredResult = result
            //    .Single(x => x.SensorTypeName == "Sensor1" &&
            //                 x.Province == "Province1" &&
            //                 x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.µg_m3));

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            //Assert.True(filteredResult.ConsistentReadings.All(
            //    x => x.Unit == Unit.µg_m3 &&
            //         x.SensorTypeName == "Sensor1" &&
            //         x.Province == "Province1"));

            //filteredResult = result
            //    .Single(x => x.SensorTypeName == "Sensor2" &&
            //                 x.Province == "Province1" &&
            //                 x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.mg_m3));

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            //Assert.True(filteredResult.ConsistentReadings.All(
            //    x => x.Unit == Unit.mg_m3 &&
            //         x.SensorTypeName == "Sensor2" &&
            //         x.Province == "Province1"));

            //filteredResult = result
            //    .Single(x => x.SensorTypeName == "Sensor3" &&
            //                 x.Province == "Province3" &&
            //                 x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.ng_m3));

            //Assert.NotNull(filteredResult);
            //Assert.Equal(2, filteredResult.ConsistentReadings.Count);
            //Assert.True(filteredResult.ConsistentReadings.All(
            //    x => x.Unit == Unit.ng_m3 &&
            //         x.SensorTypeName == "Sensor3" &&
            //         x.Province == "Province3"));

            //filteredResult = result
            //    .Single(x => x.SensorTypeName == "Sensor3" &&
            //                 x.Province == "Province2" &&
            //                 x.ConsistentReadings.Any
            //                (x => x.Unit == Unit.ng_m3));

            //Assert.NotNull(filteredResult);
            //Assert.Equal(1, filteredResult.ConsistentReadings?.Count);
            //Assert.True(filteredResult.ConsistentReadings?.All(
            //    x => x.Unit == Unit.ng_m3 &&
            //         x.SensorTypeName == "Sensor3" &&
            //         x.Province == "Province2"));
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