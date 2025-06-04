using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryTestInitializerTests
    {
        private readonly string _connectionStringTest;
        private readonly string _connectionStringMaster;
        private readonly RepositoryTestInitializer _sut;

        public RepositoryTestInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            _connectionStringMaster = config.GetConnectionString("Master");

            _sut = new RepositoryTestInitializer();
        }

        [Fact]
        public void Initialize_CreatesDatabaseAndTables_WhenDoesntExist()
        {
            // Arrange
            var databaseName = $"DBCreationTest_{Guid.NewGuid():N}";
            var connectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={databaseName};Integrated Security=True;TrustServerCertificate=True;";

            // Act
            _sut.Initialize();

            using (var masterConnection = new SqlConnection(_connectionStringMaster))
            {
                masterConnection.Open();

                var databaseExistanceResult = masterConnection.ExecuteScalar<int>
                ($"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'");

                using (var testConnection = new SqlConnection(connectionString))
                {
                    testConnection.Open();

                    var tableExistanceResult = testConnection.ExecuteScalar<int>
                        ($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                        $"WHERE TABLE_NAME = 'BasisDataTest'");

                    // Assert
                    Assert.Equal(1, databaseExistanceResult);
                    Assert.Equal(1, tableExistanceResult);
                }

                // Teardown
                masterConnection.Execute($"ALTER DATABASE [{databaseName}] " +
                                         $"SET SINGLE_USER WITH ROLLBACK " +
                                         $"IMMEDIATE; DROP DATABASE [{databaseName}]");
            }
        }
    }
}
