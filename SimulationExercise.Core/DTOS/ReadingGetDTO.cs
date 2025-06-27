using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class ReadingGetDTO
    {
        public ReadingGetDTO(long readingId, long inputFileId, byte[] bytes, Status status)
        {
            ReadingId = readingId;
            InputFileId = inputFileId;
            Bytes = bytes;
            Status = status;
        }

        public long ReadingId { get; }
        public long InputFileId { get; }
        public byte[] Bytes { get; }
        public Status Status { get; }
    }
}
