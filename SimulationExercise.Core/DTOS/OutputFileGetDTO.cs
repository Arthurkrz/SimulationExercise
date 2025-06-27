using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class OutputFileGetDTO
    {
        public OutputFileGetDTO(long outputFileId, long consistentReadingId, string name, byte[] bytes, string extension, Status status)
        {
            OutputFileId = outputFileId;
            ConsistentReadingId = consistentReadingId;
            Name = name;
            Bytes = bytes;
            Extension = extension;
            Status = status;
        }

        public long OutputFileId { get; }
        public long ConsistentReadingId { get; }
        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public Status Status { get; }
    }
}
