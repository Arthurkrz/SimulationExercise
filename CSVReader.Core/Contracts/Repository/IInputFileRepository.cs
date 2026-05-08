using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Enum;

namespace CSVReader.Core.Contracts.Repository
{
    public interface IInputFileRepository
    {
        Task InsertAsync(InputFileInsertDTO dto, IContext context);
        Task UpdateAsync(InputFileUpdateDTO dto, IContext context);
        Task<IList<InputFileGetDTO>> GetByStatusAsync(Status status, IContext context);
    }
}