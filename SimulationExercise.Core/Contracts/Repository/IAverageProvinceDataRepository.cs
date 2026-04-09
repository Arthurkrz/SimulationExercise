using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IAverageProvinceDataRepository
    {
        Task InsertAsync(AverageProvinceDataInsertDTO dto, IContext context);
        Task UpdateAsync(AverageProvinceDataUpdateDTO dto, IContext context);
        Task<IList<AverageProvinceDataGetDTO>> GetByIsExportedAsync(bool isExported, IContext context);
    }
}
