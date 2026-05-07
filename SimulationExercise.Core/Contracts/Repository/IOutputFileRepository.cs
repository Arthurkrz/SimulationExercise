using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IOutputFileRepository
    {
        Task InsertAsync(OutputFileInsertDTO dto, IContext context);

        Task UpdateAsync(OutputFileUpdateDTO dto, IContext context);

        Task<IList<OutputFileGetDTO>> GetByIsExportedAsync(bool isExported, string objectType, IContext context);
    }
}
