using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Tests.Integration.Repository;

namespace SimulationExercise.Tests.Service
{
    public class DapperRepositoryInitializerTests
    {
        private readonly string _testTableName;
        private readonly string _mainTableName;
        private readonly string _connectionStringTest;
        private readonly string _connectionStringMain;
        private readonly DapperRepositoryInitializer _sut;

        public DapperRepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.basistest.json").Build();

            _testTableName = "BasisDataTest";
            _mainTableName = "BasisData";
            _connectionStringTest = config.GetConnectionString("Test");
            _connectionStringMain = config.GetConnectionString("Main");
            _sut = new DapperRepositoryInitializer();
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
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_testTableName}'");
            }

            using (var mainConnection = new SqlConnection(_connectionStringMain))
            {
                mainConnection.Open();

                mainTableExistanceResult = mainConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_mainTableName}'");
            }

            // Assert
            Assert.Equal(1, testTableExistanceResult);
            Assert.Equal(1, mainTableExistanceResult);
        }
    }
}
