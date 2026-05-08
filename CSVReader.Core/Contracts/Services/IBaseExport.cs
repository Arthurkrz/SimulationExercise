namespace CSVReader.Core.Contracts.Services
{
    public interface IBaseExport
    {
        Task ExportAsync(string outDirectoryPath);
    }
}
