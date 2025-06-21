using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Architecture
{
    public class RepositoryInitializerTests
    {
        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameInputFileMessage = "InputFileMessage";
        private readonly IContextFactory _contextFactory;
        private readonly IRepositoryInitializer _sut;
        private readonly string _connectionString;

        public RepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(GetJsonDirectoryPath())
                .AddJsonFile("appsettings.test.json").Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);
            _sut = new RepositoryInitializer();
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
                    $"WHERE TABLE_NAME = '{_tableNameInputFile}'"));

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                    $"WHERE TABLE_NAME = '{_tableNameInputFileMessage}'"));
            }
        }

        private static string GetJsonDirectoryPath()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directoryInfo != null && !directoryInfo.GetFiles("appsettings.test.json").Any())
                directoryInfo = directoryInfo.Parent;

            return directoryInfo?.FullName ?? throw new FileNotFoundException("Configuration file not found.");
        }
    }
}
