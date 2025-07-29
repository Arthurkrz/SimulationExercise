using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Entities
{
    public class ConsistentReading
    {
        public ConsistentReading(long sensorId, string sensorTypeName, Unit unit, int value, 
                                 string province, string city, bool isHistoric, int utmNord,
                                 int utmEst, string latitude, string longitude)
        {
            SensorId = sensorId;
            SensorTypeName = sensorTypeName;
            Unit = unit;
            Value = value;
            Province = province;
            City = city;
            IsHistoric = isHistoric;
            UtmNord = utmNord;
            UtmEst = utmEst;
            Latitude = latitude;
            Longitude = longitude;
        }

        public long SensorId { get; set; }
        public string SensorTypeName { get; set; } = string.Empty;
        public Unit Unit { get; set; }
        public int Value { get; set; }
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsHistoric { get; set; }
        public int DaysOfMeasure { get; set; }
        public int UtmNord { get; set; }
        public int UtmEst { get; set; }
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
    }
}
