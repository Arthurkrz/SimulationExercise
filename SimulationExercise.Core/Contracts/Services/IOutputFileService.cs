using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IOutputFileService
    {
        Result<OutputFileInsertDTO> CreateOutputFiles<T>(IList<T> objs) where T : class;
        void Export<T>(OutputFileGetDTO obj, Stream outputStream) where T : class;
    }
}