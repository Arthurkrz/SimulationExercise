using SimulationExercise.Core.DTOS;
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
