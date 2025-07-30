using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class AverageProvinceDataGetDTO
    {
        public AverageProvinceDataGetDTO(long averageProvinceDataId,
                                         long outputFileId,
                                         string province,
                                         string sensorTypeName,
                                         double averageValue,
                                         Unit unit,
                                         int averageDaysOfMeasure,
                                         Status status)
        {
            AverageProvinceDataId = averageProvinceDataId;
            OutputFileId = outputFileId;
            Province = province;
            SensorTypeName = sensorTypeName;
            AverageValue = averageValue;
            Unit = unit;
            AverageDaysOfMeasure = averageDaysOfMeasure;
            Status = status;
        }

        public long AverageProvinceDataId { get; }
        public long OutputFileId { get; }
        public string Province { get; }
        public string SensorTypeName { get; }
        public double AverageValue { get; }
        public Unit Unit { get; }
        public int AverageDaysOfMeasure { get; }
        public Status Status { get; }
    }
}
