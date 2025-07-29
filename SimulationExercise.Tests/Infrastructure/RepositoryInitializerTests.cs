using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Architecture
{
    public class RepositoryInitializerTests
    {
        private IContextFactory _contextFactory;
        private readonly IRepositoryInitializer _sut;
        private readonly string? _connectionString;

        public RepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _connectionString = config.GetConnectionString("Default") ?? 
                throw new ArgumentNullException("Null ConnectionString");

            _contextFactory = new DapperContextFactory(_connectionString);

            _sut = new RepositoryInitializer();
        }

        [Fact]
        public void SimulationDatabaseInitializer_CreatesTables_WhenDoesntExist()
        {
            // Arrange
            string tableNameInputFile = "InputFile";
            string tableNameInputFileMessage = "InputFileMessage";

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
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameInputFile}'"));

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameInputFileMessage}'"));
            }
        }
    }
}
