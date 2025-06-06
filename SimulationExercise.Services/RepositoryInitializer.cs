using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryInitializer
    {
        private readonly string _testTableName;
        private readonly string _mainTableName;
        private readonly string _connectionStringMain;
        private readonly string _connectionStringTest;

        public RepositoryInitializer() 
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            _testTableName = "BasisDataTest";
            _mainTableName = "BasisData";
            _connectionStringMain = config.GetConnectionString("Main");
            _connectionStringTest = config.GetConnectionString("Test");
        }

        public void Initialize() => TestDatabaseTableCreationIfNotCreated(_connectionStringTest, _connectionStringMain);

        private void TestDatabaseTableCreationIfNotCreated(string testDatabaseConnectionString, string mainDatabaseConnectionString)
        {
            using (var connectionString = new SqlConnection(_connectionStringTest))
            {
                connectionString.Open();

                string testTableCreationQuery =
                    $"IF OBJECT_ID('{_testTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_testTableName} " +
                       "(BASISID BIGINT PRIMARY KEY, " +
                       "BASISCODE NVARCHAR(50) NOT NULL, " +
                       "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

                connectionString.Execute(testTableCreationQuery);
            }

            using (var connectionString = new SqlConnection(_connectionStringMain))
            {
                connectionString.Open();

                string mainTableCreationQuery =
                    $"IF OBJECT_ID('{_mainTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_mainTableName} " +
                       "(BASISID BIGINT PRIMARY KEY, " +
                       "BASISCODE NVARCHAR(50) NOT NULL, " +
                       "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

                connectionString.Execute(mainTableCreationQuery);
            }
        }
    }
}
