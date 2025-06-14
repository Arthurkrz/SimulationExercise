using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class DapperRepositoryInitializer
    {
        private readonly string _testTableName;
        private readonly string _mainTableName;
        private readonly string _connectionStringMain;
        private readonly string _connectionStringTest;

        public DapperRepositoryInitializer() 
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.basistest.json").Build();

            _testTableName = "BasisDataTest";
            _mainTableName = "BasisData";
            _connectionStringMain = config.GetConnectionString("Main");
            _connectionStringTest = config.GetConnectionString("Test");
        }

        public void Initialize() => TestBasisDatabaseTableCreationIfNotCreated(_connectionStringTest, _connectionStringMain);

        private void TestBasisDatabaseTableCreationIfNotCreated(string testDatabaseConnectionString, string mainDatabaseConnectionString)
        {
            using (var testConnection = new SqlConnection(_connectionStringTest))
            {
                testConnection.Open();

                string testTableCreationQuery =
                    $"IF OBJECT_ID('{_testTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_testTableName} " +
                       "(BASISID BIGINT PRIMARY KEY, " +
                       "BASISCODE NVARCHAR(50) NOT NULL, " +
                       "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

                testConnection.Execute(testTableCreationQuery);
            }

            using (var mainConnection = new SqlConnection(_connectionStringMain))
            {
                mainConnection.Open();

                string mainTableCreationQuery =
                    $"IF OBJECT_ID('{_mainTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_mainTableName} " +
                       "(BASISID BIGINT PRIMARY KEY, " +
                       "BASISCODE NVARCHAR(50) NOT NULL, " +
                       "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

                mainConnection.Execute(mainTableCreationQuery);
            }
        }
    }
}
