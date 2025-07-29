using FileHelpers;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    [DelimitedRecord(",")]
    public class ConsistentReadingExportDTO
    {
        public ConsistentReadingExportDTO() { }

        public ConsistentReadingExportDTO(long sensorId, string sensorTypeName, Unit unit, int value, string province, 
                                          string city, bool isHistoric, int daysOfMeasure, int utmNord, int utmEst, 
                                          string latitude, string longitude)
        {
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
        }

        public long SensorId { get; set; }
        public string? SensorTypeName { get; set; }
        public Unit Unit { get; set; }
        public int Value { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public bool IsHistoric { get; set; }
        public int DaysOfMeasure { get; set; }
        public int UtmNord { get; set; }
        public int UtmEst { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}
