namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IRepositoryInitializer
    {
        void Initialize(IContext context);
    }
}
