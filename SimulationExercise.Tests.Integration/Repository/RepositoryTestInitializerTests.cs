using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryTestInitializerTests
    {
        private readonly string _tableName;
        private readonly string _connectionString;
        private readonly RepositoryTestInitializer _sut;

        public RepositoryTestInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettingstest.json").Build();

            _tableName = "TableCreationTest";
            _connectionString = config.GetConnectionString("Test");
            _sut = new RepositoryTestInitializer();
        }

        [Fact]
        public void Initialize_CreatesTables_WhenDoesntExist()
        {
            // Act & Assert
            _sut.Initialize();

            using (var testConnection = new SqlConnection(_connectionString))
            {
                testConnection.Open();

                var tableExistanceResult = testConnection.ExecuteScalar<int>
                    ($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_tableName}'");

                Assert.Equal(1, tableExistanceResult);

                // Teardown
                testConnection.Execute("DROP TABLE dbo.TableCreationTest");
            }
        }
    }
}
