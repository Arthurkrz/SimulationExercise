using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IReadingRepository
    {
        void Insert(ReadingInsertDTO dto, IContext context);
        void Update(ReadingUpdateDTO dto, IContext context);
        IList<ReadingGetDTO> GetByStatus(Status status, IContext context);
    }
}
