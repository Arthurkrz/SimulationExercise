using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IConsistentReadingRepository
    {
        void Insert(ConsistentReadingInsertDTO dto, IContext context);
        void Update(ConsistentReadingUpdateDTO dto, IContext context);
        IList<ConsistentReadingGetDTO> GetByStatus(Status status, IContext context);
        IList<ConsistentReadingGetDTO> GetByIsExported(bool isExported, IContext context);
    }
}
