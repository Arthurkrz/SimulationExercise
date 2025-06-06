using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryInitializerTests
    {
        private readonly string _testTableName;
        private readonly string _mainTableName;
        private readonly string _connectionStringTest;
        private readonly string _connectionStringMain;
        private readonly RepositoryInitializer _sut;

        public RepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            _testTableName = "BasisDataTest";
            _mainTableName = "BasisData";
            _connectionStringTest = config.GetConnectionString("Test");
            _connectionStringMain = config.GetConnectionString("Main");
            _sut = new RepositoryInitializer();
        }

        [Fact]
        public void Initialize_CreatesTables_WhenDoesntExist()
        {
            // Arrange
            var testTableExistanceResult = 0;
            var mainTableExistanceResult = 0;

            // Act
            _sut.Initialize();

            using (var testConnection = new SqlConnection(_connectionStringTest))
            {
                testConnection.Open();

                testTableExistanceResult = testConnection.ExecuteScalar<int>
                    ($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                     $"WHERE TABLE_NAME = '{_testTableName}'");
            }

            using (var mainConnection = new SqlConnection(_connectionStringMain))
            {
                mainConnection.Open();

                testTableExistanceResult = mainConnection.ExecuteScalar<int>
                    ($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                     $"WHERE TABLE_NAME = '{_mainTableName}'");
            }

            // Assert
            Assert.Equal(1, testTableExistanceResult);
            Assert.Equal(1, mainTableExistanceResult);
        }
    }
}
