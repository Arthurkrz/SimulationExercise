using FileHelpers;

namespace SimulationExercise.Core.CSVDTOs
{
    [DelimitedRecord(",")]
    public class AverageProvinceDataExportDTO : ExportDTO
    {
        public AverageProvinceDataExportDTO() { }

        public AverageProvinceDataExportDTO(string province, 
                                            string sensorTypeName, 
                                            double averageValue, 
                                            string unit, 
                                            int averageDaysOfMeasure) 
        {
            Province = province;
            SensorTypeName = sensorTypeName;
            AverageValue = averageValue;
            Unit = unit;
            AverageDaysOfMeasure = averageDaysOfMeasure;
        }

        public string? Province { get; set; }
        public string? SensorTypeName { get; set; }
        public double AverageValue { get; set; }
        public string? Unit { get; set; }
        public int AverageDaysOfMeasure { get; set; }
    }
}
