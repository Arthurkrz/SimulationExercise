using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ReadingInsertDTO
    {
        public ReadingInsertDTO(long inputFileId, byte[] bytes, Status status)
        {
            InputFileId = inputFileId;
            Bytes = bytes;
            Status = status;
        }

        public long InputFileId { get; }
        public byte[] Bytes { get; }
        public Status Status { get; }
    }
}
