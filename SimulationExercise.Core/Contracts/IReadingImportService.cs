using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts
{
    public interface IReadingImportService
    {
        ImportResult Import(Stream stream);
    }
}
