using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IInputFileRepository
    {
        Task InsertAsync(InputFileInsertDTO dto, IContext context);
        Task UpdateAsync(InputFileUpdateDTO dto, IContext context);
        Task<IList<InputFileGetDTO>> GetByStatusAsync(Status status, IContext context);
    }
}