namespace SimulationExercise.Core.DatabaseDTOs
{
    public class AverageProvinceDataUpdateDTO
    {
        public AverageProvinceDataUpdateDTO(long averageProvinceDataId, bool isExported = false)
        {
            AverageProvinceDataId = averageProvinceDataId;
            IsExported = isExported;
        }

        public long AverageProvinceDataId { get; }
        public bool IsExported { get; set; } = false;
    }
}
