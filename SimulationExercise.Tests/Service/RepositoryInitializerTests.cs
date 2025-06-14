using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Service
{
    public class RepositoryInitializerTests
    {
        private readonly string _testInputTableName;
        private readonly string _mainInputTableName;
        private readonly string _testInputMessageTableName;
        private readonly string _mainInputMessageTableName;
        private readonly string _connectionStringTest;
        private readonly string _connectionStringMain;
        private readonly RepositoryInitializer _sut;

        public RepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json").Build();

            _testInputTableName = "InputFileTest";
            _mainInputTableName = "InputFile";
            _testInputMessageTableName = "InputFileMessageTest";
            _mainInputMessageTableName = "InputFileMessage";
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
            var testTableMessageExistanceResult = 0;
            var mainTableMessageExistanceResult = 0;

            // Act
            _sut.Initialize();

            using (var testConnection = new SqlConnection(_connectionStringTest))
            {
                testConnection.Open();

                testTableExistanceResult = testConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_testInputTableName}'");

                testTableMessageExistanceResult = testConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_testInputMessageTableName}'");
            }

            using (var mainConnection = new SqlConnection(_connectionStringMain))
            {
                mainConnection.Open();

                mainTableExistanceResult = mainConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_mainInputTableName}'");

                mainTableMessageExistanceResult = mainConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_mainInputMessageTableName}'");
            }

            // Assert
            Assert.Equal(1, testTableExistanceResult);
            Assert.Equal(1, mainTableExistanceResult);
            Assert.Equal(1, testTableMessageExistanceResult);
            Assert.Equal(1, mainTableMessageExistanceResult);
        }
    }
}
