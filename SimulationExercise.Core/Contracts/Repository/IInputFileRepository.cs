using SimulationExercise.Core.DTOS;
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