using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Entities
{
    public class AverageProvinceData
    {
        public AverageProvinceData(string province,
                                   string sensorTypeName,
                                   float averageValue,
                                   Unit unit,
                                   int averageDaysOfMeasure)
        {
            Province = province;
            SensorTypeName = sensorTypeName;
            AverageValue = averageValue;
            Unit = unit;
            AverageDaysOfMeasure = averageDaysOfMeasure;
        }

        public string Province { get; }
        public string SensorTypeName { get; }
        public float AverageValue { get; }
        public Unit Unit { get; }
        public int AverageDaysOfMeasure { get; set; }
    }
}
