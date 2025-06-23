using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingGetDTO
    {
        public ConsistentReadingGetDTO(long consistentReadingId, byte[] bytes, Status status)
        {
            ConsistentReadingId = consistentReadingId;
            Bytes = bytes;
            Status = status;
        }

        public long ConsistentReadingId { get; }
        public byte[] Bytes { get; }
        public Status Status { get; }
    }
}
