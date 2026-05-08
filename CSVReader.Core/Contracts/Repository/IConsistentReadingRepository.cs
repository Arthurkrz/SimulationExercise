using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Core.Contracts.Repository
{
    public interface IConsistentReadingRepository
    {
        Task InsertAsync(ConsistentReadingInsertDTO dto, IContext context);
        Task UpdateAsync(ConsistentReadingUpdateDTO dto, IContext context);
        Task<IList<ConsistentReadingGetDTO>> GetByIsExportedAsync(bool isExported, IContext context);
    }
}
