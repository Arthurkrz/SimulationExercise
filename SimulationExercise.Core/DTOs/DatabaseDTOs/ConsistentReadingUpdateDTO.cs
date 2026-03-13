namespace SimulationExercise.Core.DTOs.DatabaseDTOs
{
    public class ConsistentReadingUpdateDTO
    {
        public ConsistentReadingUpdateDTO(long consistentReadingId, bool isExported = false)
        {
            ConsistentReadingId = consistentReadingId;
            IsExported = isExported;
        }

        public long ConsistentReadingId { get; }
        public bool IsExported { get; set; } = false;
    }
}
