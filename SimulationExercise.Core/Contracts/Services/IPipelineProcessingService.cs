namespace SimulationExercise.Core.Contracts.Services
{
    public interface IPipelineProcessingService
    {
        void Initialize(string inDirectoryPath);
        void ProcessInputFiles();
        void ProcessReadings();
        void ProcessConsistentReadings();
    }
}
