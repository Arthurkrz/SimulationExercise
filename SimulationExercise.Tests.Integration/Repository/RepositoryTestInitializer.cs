using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryTestInitializer
    {
        private readonly string _tableName;
        private readonly string _connectionString;

        public RepositoryTestInitializer() 
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettingstest.json").Build();

            _tableName = "TableCreationTest";
            _connectionString = config.GetConnectionString("Test");
        }

        public void Initialize() => TestDatabaseTableCreationIfNotCreated(_connectionString);

        private void TestDatabaseTableCreationIfNotCreated(string databaseConnectionString)
        {
            using (var connectionString = new SqlConnection(databaseConnectionString))
            {
                connectionString.Open();

                string tableCreationQuery =
                    $"IF OBJECT_ID('{_tableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_tableName} " +
                       "(BASISID BIGINT PRIMARY KEY, " +
                       "BASISCODE NVARCHAR(50) NOT NULL, " +
                       "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

                connectionString.Execute(tableCreationQuery);
            }
        }
    }
}
