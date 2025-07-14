using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IConsistentReadingRepository
    {
        void Insert(ConsistentReadingInsertDTO dto, IContext context);
        void Update(ConsistentReadingUpdateDTO dto, IContext context);
        IList<ConsistentReadingGetDTO> GetByStatus(Status status, IContext context);
    }
}
