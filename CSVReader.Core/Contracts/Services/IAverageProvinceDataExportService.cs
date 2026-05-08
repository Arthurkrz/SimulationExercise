namespace CSVReader.Core.Contracts.Services
{
    public interface IAverageProvinceDataExportService : IBaseExport
    {
        Task CreateOutputFilesAsync();
    }
}
