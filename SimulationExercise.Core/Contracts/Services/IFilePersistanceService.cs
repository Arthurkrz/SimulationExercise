namespace SimulationExercise.Core.Contracts.Services
{
    public interface IFilePersistanceService
    {
        void Initialize(string inDirectoryPath);
        void CreateReadings();
        void CreateConsistentReadings();
        void CreateOutputFiles();
        void LoggerConfiguration(string baseOutPath);
    }
}
