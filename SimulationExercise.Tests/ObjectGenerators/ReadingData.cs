using SimulationExercise.Core.Entities;

namespace SimulationExercise.Tests.ObjectGeneration
{
    public static class ReadingData
    {
        public static IEnumerable<object[]> GetInvalidReadings()
        {
            yield return new object[]
            {
                new Reading(0, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Sensor ID less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(-1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Sensor ID less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, null, "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty sensor name." }
            };

            yield return new object[]
            {
                new Reading(1, "", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty sensor name." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ERROR", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Unit not supported." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 0,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Station ID less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", -1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Station ID less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            null, 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty station name." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty station name." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", -1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Negative value." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "mg/m³", 1,
                            "Station Name", 1, null,
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty province name." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty province name." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "mg/m³", 1,
                            "Station Name", 1, "Province",
                            null, true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty city name." }
            };

            yield return new object[]
            {
                new Reading(1, "Station Name", "µg/m³", 1,
                            "Station Name", 1, "Province",
                            "", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude"),

                new List<string> { "Null or empty city name." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            new DateTime(1967, 1, 1),
                            DateTime.Now, 1, 1,
                            "Latitude", "Longitude"),

                new List<string> { "Start date is before the possible minimum." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now,
                            DateTime.Now.AddYears(-1), 1, 1,
                            "Latitude", "Longitude"),

                new List<string> { "Stop date is before start date." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 0, 1, "Latitude", "Longitude"),

                new List<string> { "UTMNord less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, -1, 1, "Latitude", "Longitude"),

                new List<string> { "UTMNord less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 0, "Latitude", "Longitude"),

                new List<string> { "UTMEst less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, -1, "Latitude", "Longitude"),

                new List<string> { "UTMEst less or equal to 0." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, null, "Longitude"),

                new List<string> { "Null or empty latitude." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "", "Longitude"),

                new List<string> { "Null or empty latitude." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", null),

                new List<string> { "Null or empty longitude." }
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", ""),

                new List<string> { "Null or empty longitude." }
            };

            yield return new object[]
            {
                new Reading(0, null, "", 0, null, -1,
                            null, null, default,
                            new DateTime(1967, 1, 1),
                            new DateTime(1966, 1, 1),
                            0, 0, null, null),

                new List<string>
                {
                    "Sensor ID less or equal to 0.",
                    "Null or empty sensor name.",
                    "Unit not supported.",
                    "Station ID less or equal to 0.",
                    "Null or empty station name.",
                    "Negative value.",
                    "Null or empty province name.",
                    "Null or empty city name.",
                    "Start date is before the possible minimum.",
                    "Stop date is before start date.",
                    "UTMNord less or equal to 0.",
                    "UTMEst less or equal to 0.",
                    "Null or empty latitude.",
                    "Null or empty longitude."
                }
            };

            yield return new object[]
            {
                new Reading(-1, "", "ERROR", -1,
                            "", -1, "",
                            "", default,
                            new DateTime(1967, 1, 1),
                            new DateTime(1966, 1, 1),
                            -1, -1, "", ""),

                new List<string>
                {
                    "Sensor ID less or equal to 0.",
                    "Null or empty sensor name.",
                    "Unit not supported.",
                    "Station ID less or equal to 0.",
                    "Null or empty station name.",
                    "Negative value.",
                    "Null or empty province name.",
                    "Null or empty city name.",
                    "Start date is before the possible minimum.",
                    "Stop date is before start date.",
                    "UTMNord less or equal to 0.",
                    "UTMEst less or equal to 0.",
                    "Null or empty latitude.",
                    "Null or empty longitude."
                }
            };
        }

        public static IEnumerable<object[]> GetValidReadings()
        {
            yield return new object[]
            {
                new Reading(1, "Sensor Name", "ng/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude")
            };
            yield return new object[]
            {
                new Reading(1, "Sensor Name", "mg/m³", 1,
                            "Station Name", 0, "Province",
                            "City", false,
                            DateTime.Now.AddYears(-1),
                            DateTime.Now, 1, 1, "Latitude", "Longitude")
            };

            yield return new object[]
            {
                new Reading(1, "Sensor Name", "µg/m³", 1,
                            "Station Name", 1, "Province",
                            "City", true,
                            DateTime.Now.AddYears(-1),
                            null, 1, 1, "Latitude", "Longitude")
            };
        }
    }
}
