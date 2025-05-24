using Microsoft.Data.SqlClient;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Architecture
{
    public class DapperContextFactory : IContextFactory
    {
        private readonly string _connectionString;

        public DapperContextFactory(string connectionString)
        {
            this._connectionString = connectionString ?? throw new 
                ArgumentNullException(nameof(connectionString));
        }

        public IContext Create()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            return new DapperContext(transaction);
        }
    }
}
