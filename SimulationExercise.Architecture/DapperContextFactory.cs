using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Core.Contracts.Infrastructure;

namespace SimulationExercise.Infrastructure
{
    public class DapperContextFactory : IContextFactory
    {
        private readonly string _connectionString;

        public DapperContextFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentNullException("Null ConnectionString");
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
