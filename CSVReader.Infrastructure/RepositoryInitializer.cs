using CSVReader.Core.Contracts.Infrastructure;

namespace CSVReader.Infrastructure
{
    public class RepositoryInitializer : IRepositoryInitializer
    {
        public void Initialize(IContext context) =>
            SimulationDatabaseInitializer(context);

        private void SimulationDatabaseInitializer(IContext context)
        {
            IList<string> queryList = SeedDatabase.GetSimulationDatabaseQueries();
            foreach (var query in queryList) context.Execute(query);
            context.Commit();
        }
    }
}
