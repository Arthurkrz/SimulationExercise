using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class AverageProvinceDataGetDTO
    {
        public AverageProvinceDataGetDTO(long averageProvinceDataId,
                                         string province,
                                         string sensorTypeName,
                                         double averageValue,
                                         Unit unit,
                                         int averageDaysOfMeasure,
                                         bool isExported)
        {
            AverageProvinceDataId = averageProvinceDataId;
            Province = province;
            SensorTypeName = sensorTypeName;
            AverageValue = averageValue;
            Unit = unit;
            AverageDaysOfMeasure = averageDaysOfMeasure;
            IsExported = isExported;
        }

        public long AverageProvinceDataId { get; }
        public string Province { get; }
        public string SensorTypeName { get; }
        public double AverageValue { get; }
        public Unit Unit { get; }
        public int AverageDaysOfMeasure { get; }
        public bool IsExported { get; set; } = false;
    }
}
