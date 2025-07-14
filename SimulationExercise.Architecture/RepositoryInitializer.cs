using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Infrastructure
{
    public class RepositoryInitializer : IRepositoryInitializer
    {
        public void Initialize(IContext context)
        {
            var connectionString = context.GetConnectionString();
            if (connectionString.Contains("Simulation_Database"))
                SimulationDatabaseInitializer(context);
            else BasisInitializer(context);
        }

        private void BasisInitializer(IContext context)
        {
            IList<string> queryList = TableCreationQueryGenerator.GetBasisQueries();
            foreach (var query in queryList) context.Execute(query);
            context.Commit();
        }

        private void SimulationDatabaseInitializer(IContext context)
        {
            IList<string> queryList = TableCreationQueryGenerator.GetSimulationDatabaseQueries();
            foreach (var query in queryList) context.Execute(query);
            context.Commit();
        }
    }
}
