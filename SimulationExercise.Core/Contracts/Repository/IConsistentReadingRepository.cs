using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IConsistentReadingRepository
    {
        void Insert(ConsistentReadingInsertDTO dto, IContext context);
        void Update(ConsistentReadingUpdateDTO dto, IContext context);
        IList<ConsistentReadingGetDTO> GetByIsExported(bool isExported, IContext context);
    }
}
