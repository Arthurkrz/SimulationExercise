using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;

namespace CSVReader.Core.Contracts.Services
{
    public interface IOutputFileService
    {
        Task<Result<OutputFileInsertDTO>> CreateOutputFilesAsync<T>(IList<T> objs) where T : class;
    }
}