namespace SimulationExercise.Core.DTOs.DatabaseDTOs
{
    public class OutputFileUpdateDTO
    {
        public OutputFileUpdateDTO(long outputFileId, bool isExported)
        {
            OutputFileId = outputFileId;
            IsExported = isExported;
        }

        public long OutputFileId { get; }
        public bool IsExported { get; set; } = false;
    }
}
