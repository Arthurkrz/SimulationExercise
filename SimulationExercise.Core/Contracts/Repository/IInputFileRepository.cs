using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IInputFileRepository
    {
        void Insert(InputFileInsertDTO dto, IContext context);
        void Update(InputFileUpdateDTO dto, IContext context);
        IList<InputFileGetDTO> GetByStatus(Status status, IContext context);
    }
}