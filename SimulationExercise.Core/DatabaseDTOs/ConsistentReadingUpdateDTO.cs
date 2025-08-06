using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingUpdateDTO
    {
        public ConsistentReadingUpdateDTO(long consistentReadingId, Status status, bool isExported = false, IList<string>? messages = null)
        {
            if (status == Status.Error && (messages == null || !messages.Any()))
                throw new ArgumentNullException("Update as error without error message list is not allowed.");

            else if (status == Status.Success && messages != null)
                throw new ArgumentException("Update as success with error message list is not allowed.");

            ConsistentReadingId = consistentReadingId;
            IsExported = isExported;
            Status = status;
            Messages = messages ?? new List<string>();
        }

        public long ConsistentReadingId { get; }
        public bool IsExported { get; set; } = false;
        public Status Status { get; }
        public IList<string> Messages { get; }
    }
}
