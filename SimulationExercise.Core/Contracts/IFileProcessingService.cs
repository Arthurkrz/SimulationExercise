namespace SimulationExercise.Core.Contracts
{
    public interface IFileProcessingService
    {
        void ProcessFile(string inFilePath, string outFilePath);
    }
}
