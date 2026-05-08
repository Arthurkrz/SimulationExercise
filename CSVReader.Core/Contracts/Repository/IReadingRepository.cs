using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Enum;

namespace CSVReader.Core.Contracts.Repository
{
    public interface IReadingRepository
    {
        Task InsertAsync(ReadingInsertDTO dto, IContext context);
        Task UpdateAsync(ReadingUpdateDTO dto, IContext context);
        Task<IList<ReadingGetDTO>> GetByStatusAsync(Status status, IContext context);
    }
}
