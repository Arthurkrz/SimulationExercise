using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IOutputFileRepository
    {
        void Insert(OutputFileInsertDTO dto, IContext context);
        IList<OutputFileGetDTO> GetByObjectType(Type objectType, IContext context);
        IList<OutputFileGetDTO> GetByIsExported(bool isExported, IContext context);
    }
}
