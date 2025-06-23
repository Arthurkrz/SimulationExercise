using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Architecture.Repository
{
    public class OutputFileRepository : IOutputFileRepository
    {
        public IList<OutputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Insert(OutputFileInsertDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }

        public void Update(OutputFileUpdateDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
