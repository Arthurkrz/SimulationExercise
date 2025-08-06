namespace SimulationExercise.Core.Contracts.Services
{
    public interface IFilePersistanceService
    {
        void Initialize(string inDirectoryPath);
        void CreateReadings();
        void CreateConsistentReadings();
        void CreateAverageProvinceDatas();
        void CreateAverageProvinceDataOutputFiles();
        void CreateConsistentReadingOutputFiles();
        void ExportAverageProvinceData(string outDirectoryPath);
        void ExportConsistentReadings(string outDirectoryPath);
        void LoggerConfiguration(string baseOutPath);
    }
}
