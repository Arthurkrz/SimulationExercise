using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class OutputFileUpdateDTO
    {
        public OutputFileUpdateDTO(long outputFileId, Status status, IList<string> messages)
        {
            OutputFileId = outputFileId;
            Status = status;
            Messages = messages;
        }

        public long OutputFileId { get; }
        public Status Status { get; }
        public IList<string> Messages { get; }
    }
}
