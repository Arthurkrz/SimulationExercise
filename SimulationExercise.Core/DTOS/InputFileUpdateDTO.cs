using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class InputFileUpdateDTO
    {
        public InputFileUpdateDTO(long inputFileId, Status status, IList<string> messages)
        {
            InputFileId = inputFileId;
            Status = status;
            Messages = messages;
        }

        public long InputFileId { get; }
        public Status Status { get; }
        public IList<string> Messages { get; }
    }
}
