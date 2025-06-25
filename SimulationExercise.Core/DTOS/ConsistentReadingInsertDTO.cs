using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingInsertDTO
    {
        public ConsistentReadingInsertDTO(long readingId, byte[] bytes, Status status)
        {
            ReadingId = readingId;
            Bytes = bytes;
            Status = status;
        }

        public long ReadingId { get; }
        public byte[] Bytes { get; }
        public Status Status { get; }
    }
}
