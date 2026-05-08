namespace CSVReader.Core.Contracts.Services
{
    public interface IInputFileService
    {
        Task ProcessFilesAsync(string inDirectoryPath);
    }
}
