using Bogus;
using SimulationExercise.Core;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Tests
{
    public static class ProvinceDataData
    {
        public static IEnumerable<object[]> GetInconsistentProvinceData()
        {
            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(1, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(2, "Sensor2", Unit.mg_m3, 110, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List<string> { "Inconsistent sensor names in readings." }
            };

            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(3, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(4, "Sensor1", Unit.ng_m3, 110, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List<string> { "Inconsistent units in readings." }
            };

            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(5, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(6, "Sensor1", Unit.mg_m3, 110, "Province2", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List <string> { "Inconsistent provinces in readings." }
            };


            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(7, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(8, "Sensor2", Unit.ng_m3, 110, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List <string>
                { "Inconsistent units in readings.",
                  "Inconsistent sensor names in readings." }
            };

            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(9, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(10, "Sensor2", Unit.mg_m3, 110, "Province2", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List <string>
                { "Inconsistent provinces in readings.",
                  "Inconsistent sensor names in readings." }
            };

            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(11, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(12, "Sensor1", Unit.ng_m3, 110, "Province2", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List <string>
                { "Inconsistent provinces in readings.",
                  "Inconsistent units in readings." }
            };

            yield return new object[]
            {
                new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
                {
                    new ConsistentReading(13, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 },
                    new ConsistentReading(14, "Sensor2", Unit.ng_m3, 110, "Province2", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }
                }),

                new List <string>
                { "Inconsistent provinces in readings.",
                  "Inconsistent units in readings.",
                  "Inconsistent sensor names in readings." }
            };
        }

        public static IEnumerable<object[]> GetProvinceData()
        {
            Faker faker = new Faker();
            List<int> randomValues = new List<int>
            {
                faker.Random.Int(1, 10000),
                faker.Random.Int(1, 10000),
                faker.Random.Int(1, 10000)
            };

            List<int> randomDaysOfMeasure = new List<int>
            {
                faker.Random.Int(1, 1000),
                faker.Random.Int(1, 1000),
                faker.Random.Int(1, 1000)
            };

            double averageValue = randomValues.Average();
            int averageDaysOfMeasure = (int)randomDaysOfMeasure.Average();

            yield return new object[]
            {
               new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
               {
                   new ConsistentReading(1, "Sensor1", Unit.mg_m3, randomValues[0], "Province1", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(2, "Sensor1", Unit.mg_m3, randomValues[1], "Province1", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(3, "Sensor1", Unit.mg_m3, randomValues[2], "Province1", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.mg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
               {
                   new ConsistentReading(3, "Sensor1", Unit.ng_m3, randomValues[0], "Province1", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(4, "Sensor1", Unit.ng_m3, randomValues[1], "Province1", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(5, "Sensor1", Unit.ng_m3, randomValues[2], "Province1", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.ng_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province1", "Sensor1", new List<ConsistentReading>
               {
                   new ConsistentReading(5, "Sensor1", Unit.µg_m3, randomValues[0], "Province1", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(6, "Sensor1", Unit.µg_m3, randomValues[1], "Province1", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(7, "Sensor1", Unit.µg_m3, randomValues[2], "Province1", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.µg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province1", "Sensor2", new List<ConsistentReading>
               {
                   new ConsistentReading(7, "Sensor2", Unit.mg_m3, randomValues[0], "Province1", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(8, "Sensor2", Unit.mg_m3, randomValues[1], "Province1", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(9, "Sensor2", Unit.mg_m3, randomValues[2], "Province1", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.mg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province1", "Sensor2", new List<ConsistentReading>
               {
                   new ConsistentReading(9, "Sensor2", Unit.ng_m3, randomValues[0], "Province1", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(10, "Sensor2", Unit.ng_m3, randomValues[1], "Province1", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(11, "Sensor2", Unit.ng_m3, randomValues[2], "Province1", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.ng_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province1", "Sensor2", new List<ConsistentReading>
               {
                   new ConsistentReading(11, "Sensor2", Unit.µg_m3, randomValues[0], "Province1", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(12, "Sensor2", Unit.µg_m3, randomValues[1], "Province1", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(13, "Sensor2", Unit.µg_m3, randomValues[2], "Province1", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.µg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province2", "Sensor1", new List<ConsistentReading>
               {
                   new ConsistentReading(13, "Sensor1", Unit.mg_m3, randomValues[0], "Province2", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(14, "Sensor1", Unit.mg_m3, randomValues[1], "Province2", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(15, "Sensor1", Unit.mg_m3, randomValues[2], "Province2", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.mg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province2", "Sensor1", new List<ConsistentReading>
               {
                   new ConsistentReading(15, "Sensor1", Unit.ng_m3, randomValues[0], "Province2", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(16, "Sensor1", Unit.ng_m3, randomValues[1], "Province2", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(17, "Sensor1", Unit.ng_m3, randomValues[2], "Province2", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.ng_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province2", "Sensor1", new List<ConsistentReading>
               {
                   new ConsistentReading(17, "Sensor1", Unit.µg_m3, randomValues[0], "Province2", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(18, "Sensor1", Unit.µg_m3, randomValues[1], "Province2", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(19, "Sensor1", Unit.µg_m3, randomValues[2], "Province2", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.µg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province2", "Sensor2", new List<ConsistentReading>
               {
                   new ConsistentReading(19, "Sensor2", Unit.mg_m3, randomValues[0], "Province2", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(20, "Sensor2", Unit.mg_m3, randomValues[1], "Province2", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(21, "Sensor2", Unit.mg_m3, randomValues[2], "Province2", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.mg_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province2", "Sensor2", new List<ConsistentReading>
               {
                   new ConsistentReading(21, "Sensor2", Unit.ng_m3, randomValues[0], "Province2", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(22, "Sensor2", Unit.ng_m3, randomValues[1], "Province2", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(23, "Sensor2", Unit.ng_m3, randomValues[2], "Province2", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.ng_m3,

               averageDaysOfMeasure
            };

            yield return new object[]
            {
               new ProvinceData("Province2", "Sensor2", new List<ConsistentReading>
               {
                   new ConsistentReading(23, "Sensor2", Unit.µg_m3, randomValues[0], "Province2", "City1", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[0] },
                   new ConsistentReading(24, "Sensor2", Unit.µg_m3, randomValues[1], "Province2", "City2", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[1] },
                   new ConsistentReading(25, "Sensor2", Unit.µg_m3, randomValues[2], "Province2", "City3", false, 123, 456, "45.0", "9.0") { DaysOfMeasure = randomDaysOfMeasure[2] }
               }),

               averageValue,

               Unit.µg_m3,

               averageDaysOfMeasure
            };
        }

    }
}
