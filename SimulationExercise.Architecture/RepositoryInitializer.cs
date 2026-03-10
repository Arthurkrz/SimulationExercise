using SimulationExercise.Core.Contracts.Infrastructure;

namespace SimulationExercise.Infrastructure
{
    public class RepositoryInitializer : IRepositoryInitializer
    {
        public void Initialize(IContext context)
        {
            var connectionString = context.GetConnectionString();
            SimulationDatabaseInitializer(context);
        }

        private void SimulationDatabaseInitializer(IContext context)
        {
            IList<string> queryList = SeedDatabase.GetSimulationDatabaseQueries();
            foreach (var query in queryList) context.Execute(query);
            context.Commit();
        }
    }
}
