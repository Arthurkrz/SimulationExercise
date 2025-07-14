using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class OutputFileInsertDTO
    {
        public OutputFileInsertDTO(string name, byte[] bytes, string extension, Status status)
        {
            Name = name;
            Bytes = bytes;
            Extension = extension;
            Status = status;
        }

        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public Status Status { get; }
    }
}
