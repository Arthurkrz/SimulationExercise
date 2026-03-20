namespace SimulationExercise.Core.DTOs.DatabaseDTOs
{
    public class AverageProvinceDataUpdateDTO
    {
        public AverageProvinceDataUpdateDTO(long averageProvinceDataId, bool isExported)
        {
            AverageProvinceDataId = averageProvinceDataId;
            IsExported = isExported;
        }

        public long AverageProvinceDataId { get; }
        public bool IsExported { get; set; } = false;
    }
}
