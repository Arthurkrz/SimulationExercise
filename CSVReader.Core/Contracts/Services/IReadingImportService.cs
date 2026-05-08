using CSVReader.Core.Entities;

namespace CSVReader.Core.Contracts.Services
{
    public interface IReadingImportService
    {
        ImportResult Import(Stream stream);
    }
}
