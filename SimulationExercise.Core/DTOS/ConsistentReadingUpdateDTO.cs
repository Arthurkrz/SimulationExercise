using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingUpdateDTO
    {
        public ConsistentReadingUpdateDTO(long consistentReadingId, Status status, IList<string> messages)
        {
            ConsistentReadingId = consistentReadingId;
            Status = status;
            Messages = messages;
        }

        public long ConsistentReadingId { get; }
        public Status Status { get; }
        public IList<string> Messages { get; }
    }
}
