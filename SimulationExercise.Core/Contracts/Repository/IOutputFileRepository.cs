using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IOutputFileRepository
    {
        Task InsertAsync(OutputFileInsertDTO dto, IContext context);
        Task<IList<OutputFileGetDTO>> GetByObjectTypeAsync(Type objectType, IContext context);
        Task<IList<OutputFileGetDTO>> GetByIsExportedAsync(bool isExported, IContext context);
    }
}
