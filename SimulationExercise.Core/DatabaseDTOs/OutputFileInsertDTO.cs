namespace SimulationExercise.Core.DTOS
{
    public class OutputFileInsertDTO
    {
        public OutputFileInsertDTO(string name, byte[] bytes, string extension, Type objectType, bool isExported)
        {
            Name = name;
            Bytes = bytes;
            Extension = extension;
            ObjectType = objectType;
            IsExported = isExported;
        }

        public string Name { get; }
        public byte[] Bytes { get; }
        public string Extension { get; }
        public Type ObjectType { get; }
        public bool IsExported { get; }
    }
}
