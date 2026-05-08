namespace CSVReader.Core.Contracts.Services
{
    public interface IConsistentReadingExportService : IBaseExport
    {
        Task CreateOutputFilesAsync();
    }
}
