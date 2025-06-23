using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ReadingUpdateDTO
    {
        public ReadingUpdateDTO(long readingId, Status status, IList<string> messages)
        {
            ReadingId = readingId;
            Status = status;
            Messages = messages;
        }

        public long ReadingId { get; }
        public Status Status { get; }
        public IList<string> Messages { get; }
    }
}
