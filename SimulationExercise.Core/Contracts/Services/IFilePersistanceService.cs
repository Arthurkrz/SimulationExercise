using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IFilePersistanceService
    {
        void Initialize(string inDirectoryPath);
    }
}
