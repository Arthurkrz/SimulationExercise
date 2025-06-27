using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingGetDTO
    {
        public ConsistentReadingGetDTO(long consistentReadingId, long readingId, byte[] bytes, Status status)
        {
            ConsistentReadingId = consistentReadingId;
            ReadingId = readingId;
            Bytes = bytes;
            Status = status;
        }

        public long ConsistentReadingId { get; }
        public long ReadingId { get; }
        public byte[] Bytes { get; }
        public Status Status { get; }
    }
}
