using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Architecture
{
    public class DapperRepositoryInitializerTests
    {
        private readonly string _tableName = "BasisData";
        private readonly IContextFactory _contextFactory;
        private readonly DapperRepositoryInitializer _sut;
        private readonly string _connectionString;

        public DapperRepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(GetJsonDirectoryPath())
                .AddJsonFile("appsettings.basistest.json").Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);
            _sut = new DapperRepositoryInitializer();
        }
        
        [Fact]
        public void Initialize_CreatesTables_WhenDoesntExist()
        {
            // Arrange
            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Initialize(context);
            }

            using (var assertConnection = new SqlConnection(_connectionString))
            {
                assertConnection.Open();

                // Assert
                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_tableName}'"));
            }
        }

        private static string GetJsonDirectoryPath()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directoryInfo != null && !directoryInfo.GetFiles("appsettings.basistest.json").Any())
                directoryInfo = directoryInfo.Parent;

            return directoryInfo?.FullName ?? throw new FileNotFoundException("Configuration file not found.");
        }
    }
}
