using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class OutputFileInsertDTO
    {
        public OutputFileInsertDTO(string name, byte[] bytes, string extension, OutputObjectType objectType, bool isAPDExported, Status status)
        {
            Name = name;
            Bytes = bytes;
            Extension = extension;
            Status = status;
            ObjectType = objectType;
            IsAverageProvinceDataExported = isAPDExported;
        }

        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public OutputObjectType ObjectType { get; }
        public bool IsAverageProvinceDataExported { get; } = false;
        public Status Status { get; }
    }
}
