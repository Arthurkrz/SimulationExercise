using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IOutputFileService
    {
        Task<Result<OutputFileInsertDTO>> CreateOutputFilesAsync<T>(IList<T> objs) where T : class;
        void Export<T>(T obj, Stream outputStream) where T : class;
    }
}