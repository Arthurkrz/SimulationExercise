namespace SimulationExercise.Core.DTOS
{
    public class ReadingInsertDTO
    {
        public ReadingInsertDTO(byte[] bytes, string extension)
        {
            Bytes = bytes;
            Extension = extension;
        }

        public byte[] Bytes { get; }
        public string Extension { get; }
    }
}
