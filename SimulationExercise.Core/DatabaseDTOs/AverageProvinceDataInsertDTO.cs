using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class AverageProvinceDataInsertDTO
    {
        public AverageProvinceDataInsertDTO(string province,
                                            string sensorTypeName,
                                            double averageValue,
                                            Unit unit,
                                            int averageDaysOfMeasure,
                                            bool isExported)
        {
            Province = province;
            SensorTypeName = sensorTypeName;
            AverageValue = averageValue;
            Unit = unit;
            AverageDaysOfMeasure = averageDaysOfMeasure;
            IsExported = isExported;
        }

        public string Province { get; }
        public string SensorTypeName { get; }
        public double AverageValue { get; }
        public Unit Unit { get; }
        public int AverageDaysOfMeasure { get; }
        public bool IsExported { get; set; }
    }
}
