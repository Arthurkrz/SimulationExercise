using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Architecture.Repository
{
    public class ReadingRepository : IReadingRepository
    {
        public IList<ReadingGetDTO> GetByStatus(Status status, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Insert(ReadingInsertDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Update(ReadingUpdateDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
