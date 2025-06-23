using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ConsistentReadingInsertDTO
    {
        public ConsistentReadingInsertDTO(byte[] bytes, Status status)
        {
            Bytes = bytes;
            Status = status;
        }

        public byte[] Bytes { get; }
        public Status Status { get; }
    }
}
