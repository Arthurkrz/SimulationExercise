using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CSVReader.Core.Contracts.Infrastructure;

namespace CSVReader.Infrastructure
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
