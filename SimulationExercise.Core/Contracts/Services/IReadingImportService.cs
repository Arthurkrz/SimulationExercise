using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IReadingImportService
    {
        ImportResult Import(Stream stream);
    }
}
