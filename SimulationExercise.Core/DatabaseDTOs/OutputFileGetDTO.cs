using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class OutputFileGetDTO
    {
        public OutputFileGetDTO(long outputFileId, string name, byte[] bytes, string extension, OutputObjectType objectType, bool isAPDExported, Status status)
        {
            OutputFileId = outputFileId;
            Name = name;
            Bytes = bytes;
            Extension = extension;
            ObjectType = objectType;
            IsAverageProvinceDataExported = isAPDExported;
            Status = status;
        }

        public long OutputFileId { get; }
        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public OutputObjectType ObjectType { get; }
        public bool IsAverageProvinceDataExported { get; } = false;
        public Status Status { get; }
    }
}
