using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Core.Contracts.Repository
{
    public interface IOutputFileRepository
    {
        Task InsertAsync(OutputFileInsertDTO dto, IContext context);

        Task UpdateAsync(OutputFileUpdateDTO dto, IContext context);

        Task<IList<OutputFileGetDTO>> GetByIsExportedAsync(bool isExported, string objectType, IContext context);
    }
}
