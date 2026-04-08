using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IReadingRepository
    {
        Task InsertAsync(ReadingInsertDTO dto, IContext context);
        Task UpdateAsync(ReadingUpdateDTO dto, IContext context);
        Task<IList<ReadingGetDTO>> GetByStatusAsync(Status status, IContext context);
    }
}
