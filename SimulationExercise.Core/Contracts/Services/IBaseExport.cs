namespace SimulationExercise.Core.Contracts.Services
{
    public interface IBaseExport
    {
        Task ExportAsync(string outDirectoryPath);
    }
}
