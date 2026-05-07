namespace SimulationExercise.Core.Contracts.Services
{
    public interface IAverageProvinceDataExportService : IBaseExport
    {
        Task CreateOutputFilesAsync();
    }
}
