using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class InputFileGetDTO
    {
        public InputFileGetDTO(long inputFileId, string name, byte[] bytes, string extension, Status status)
        {
            InputFileId = inputFileId;
            Name = name;
            Bytes = bytes;
            Extension = extension;
            Status = status;
        }

        public long InputFileId { get; }
        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public Status Status { get; }
    }
}
