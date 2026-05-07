namespace SimulationExercise.Core.DTOs.DatabaseDTOs
{
    public class OutputFileGetDTO
    {
        public OutputFileGetDTO(long outputFileId, string name, byte[] bytes, string extension, string objectType, bool isExported)
        {
            OutputFileId = outputFileId;
            Name = name;
            Bytes = bytes;
            Extension = extension;
            ObjectType = objectType;
            IsExported = isExported;
        }

        public long OutputFileId { get; }
        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public string ObjectType { get; }
        public bool IsExported { get; }
    }
}
