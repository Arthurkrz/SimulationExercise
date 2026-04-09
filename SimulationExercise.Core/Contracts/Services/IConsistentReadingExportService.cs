namespace SimulationExercise.Core.Contracts.Services
{
    public interface IConsistentReadingExportService : IBaseExport
    {
        Task CreateOutputFilesAsync();
    }
}
