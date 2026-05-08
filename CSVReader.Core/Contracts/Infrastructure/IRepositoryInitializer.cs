namespace CSVReader.Core.Contracts.Infrastructure
{
    public interface IRepositoryInitializer
    {
        void Initialize(IContext context);
    }
}
