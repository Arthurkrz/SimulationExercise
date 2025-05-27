namespace SimulationExercise.Core.Contracts.Services
{
    public interface IFileProcessingService
    {
        void ProcessFile(string inFilePath, string outFilePath);
    }
}
