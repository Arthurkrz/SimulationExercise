using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IInputFileService
    {
        void ProcessFiles(string inDirectoryPath);
    }
}
