using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Architecture
{
    public class RepositoryInitializerTests
    {
        private IContextFactory _contextFactory;
        private readonly IRepositoryInitializer _sut;
        private readonly string _connectionStringMain;
        private readonly string _connectionStringBasis;

        public RepositoryInitializerTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _connectionStringMain = config.GetConnectionString("DefaultDatabase");
            _connectionStringBasis = config.GetConnectionString("BasisDatabase");
            _sut = new RepositoryInitializer();
        }

        [Fact]
        public void SimulationDatabaseInitializer_CreatesTables_WhenDoesntExist()
        {
            // Arrange
            string tableNameInputFile = "InputFile";
            string tableNameInputFileMessage = "InputFileMessage";

            _contextFactory = new DapperContextFactory(_connectionStringMain);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Initialize(context);
            }

            using (var assertConnection = new SqlConnection(_connectionStringMain))
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

        [Fact]
        public void BasisInitializer_CreatesTables_WhenDoesntExist()
        {
            // Arrange
            string tableName = "BasisData";

            _contextFactory = new DapperContextFactory(_connectionStringBasis);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Initialize(context);
            }

            using (var assertConnection = new SqlConnection(_connectionStringBasis))
            {
                assertConnection.Open();

                // Assert
                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableName}'"));
            }
        }
    }
}
