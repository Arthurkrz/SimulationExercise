namespace SimulationExercise.Core.Contracts.Services
{
    public interface IFilePersistanceService
    {
        Task Initialize(string inDirectoryPath);
        Task CreateReadings();
        Task CreateConsistentReadings();
        Task CreateAverageProvinceDatas();
        Task CreateAverageProvinceDataOutputFiles();
        Task CreateConsistentReadingOutputFiles();
        Task ExportAverageProvinceData(string outDirectoryPath);
        Task ExportConsistentReadings(string outDirectoryPath);
        bool LoggerConfiguration(string baseOutPath);
    }
}
