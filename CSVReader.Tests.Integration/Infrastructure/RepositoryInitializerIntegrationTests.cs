using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CSVReader.Infrastructure;
using CSVReader.Core.Contracts.Infrastructure;

namespace CSVReader.Tests.Integration.Infrastructure
{
    public class RepositoryInitializerIntegrationTests
    {
        private IContextFactory _contextFactory;
        private readonly IRepositoryInitializer _sut;
        private readonly string? _connectionString;

        private readonly string tableNameInputFile = "InputFile";
        private readonly string tableNameInputFileMessage = "InputFileMessage";

        private readonly string tableNameReading = "Reading";
        private readonly string tableNameReadingMessage = "ReadingMessage";

        private readonly string tableNameConsistentReading = "ConsistentReading";

        private readonly string tableNameOutputFile = "OutputFile";

        private readonly string tableNameAverageProvinceData = "AverageProvinceData";

        public RepositoryInitializerIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _connectionString = config.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException("Null ConnectionString");

            _contextFactory = new DapperContextFactory(config);

            _sut = new RepositoryInitializer();
        }

        [Fact]
        public void SimulationDatabaseInitializer_CreatesTables_WhenDoesntExist()
        {
            using (var arrangeConnection = new SqlConnection(_connectionString))
            {
                // Arrange
                arrangeConnection.Open();

                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameConsistentReading}', 'U') IS NOT NULL DROP TABLE {tableNameConsistentReading};");
                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameOutputFile}', 'U') IS NOT NULL DROP TABLE {tableNameOutputFile};");
                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameAverageProvinceData}', 'U') IS NOT NULL DROP TABLE {tableNameAverageProvinceData};");
                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameReadingMessage}', 'U') IS NOT NULL DROP TABLE {tableNameReadingMessage};");
                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameReading}', 'U') IS NOT NULL DROP TABLE {tableNameReading};");
                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameInputFileMessage}', 'U') IS NOT NULL DROP TABLE {tableNameInputFileMessage};");
                arrangeConnection.Execute($@"IF OBJECT_ID('{tableNameInputFile}', 'U') IS NOT NULL DROP TABLE {tableNameInputFile};");
            }

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

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameReading}'"));

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameReadingMessage}'"));

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameConsistentReading}'"));

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameOutputFile}'"));

                Assert.Equal(1, assertConnection.ExecuteScalar<int>
                    ($@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = '{tableNameAverageProvinceData}'"));
            }
        }
    }
}
