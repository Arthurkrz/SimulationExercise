namespace SimulationExercise.Core.Contracts
{
    public interface IReadingImportService
    {
        ImportResult Import(Stream stream);
    }
}
