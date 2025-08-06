using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingGetDTO
    {
        public ConsistentReadingGetDTO(long consistentReadingId, long readingId, long sensorId, string sensorTypeName, 
                                       Unit unit, int value, string province, string city, bool isHistoric, 
                                       int daysOfMeasure, int utmNord, int utmEst, string latitude, 
                                       string longitude, bool isExported, Status status)
        {
            ConsistentReadingId = consistentReadingId;
            ReadingId = readingId;
            SensorId = sensorId;
            SensorTypeName = sensorTypeName;
            Unit = unit;
            Value = value;
            Province = province;
            City = city;
            IsHistoric = isHistoric;
            DaysOfMeasure = daysOfMeasure;
            UtmNord = utmNord;
            UtmEst = utmEst;
            Latitude = latitude;
            Longitude = longitude;
            IsExported = isExported;
            Status = status;
        }

        public long ConsistentReadingId { get; }
        public long ReadingId { get; }
        public long SensorId { get; set; }
        public string SensorTypeName { get; set; }
        public Unit Unit { get; set; }
        public int Value { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public bool IsHistoric { get; set; }
        public int DaysOfMeasure { get; set; }
        public int UtmNord { get; set; }
        public int UtmEst { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsExported { get; set; }
        public Status Status { get; }
    }
}
