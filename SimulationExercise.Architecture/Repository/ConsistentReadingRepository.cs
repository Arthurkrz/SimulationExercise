using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Architecture.Repository
{
    public class ConsistentReadingRepository : IConsistentReadingRepository
    {
        public IList<ConsistentReadingGetDTO> GetByStatus(Status status, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Insert(ConsistentReadingInsertDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Update(ConsistentReadingUpdateDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
