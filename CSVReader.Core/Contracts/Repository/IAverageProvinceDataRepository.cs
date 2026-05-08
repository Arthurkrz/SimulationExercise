using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Core.Contracts.Repository
{
    public interface IAverageProvinceDataRepository
    {
        Task InsertAsync(AverageProvinceDataInsertDTO dto, IContext context);
        Task UpdateAsync(AverageProvinceDataUpdateDTO dto, IContext context);
        Task<IList<AverageProvinceDataGetDTO>> GetByIsExportedAsync(bool isExported, IContext context);
    }
}
