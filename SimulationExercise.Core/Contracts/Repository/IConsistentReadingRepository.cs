using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IConsistentReadingRepository
    {
        Task InsertAsync(ConsistentReadingInsertDTO dto, IContext context);
        Task UpdateAsync(ConsistentReadingUpdateDTO dto, IContext context);
        Task<IList<ConsistentReadingGetDTO>> GetByIsExportedAsync(bool isExported, IContext context);
    }
}
