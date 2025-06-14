using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimulationExercise.Services
{
    public class RepositoryInitializer
    {
        private readonly string _testInputTableName;
        private readonly string _mainInputTableName;
        private readonly string _testInputMessageTableName;
        private readonly string _mainInputMessageTableName;
        private readonly string _connectionStringMain;
        private readonly string _connectionStringTest;

        public RepositoryInitializer()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json").Build();

            _testInputTableName = "InputFileTest";
            _mainInputTableName = "InputFile";
            _testInputMessageTableName = "InputFileMessageTest";
            _mainInputMessageTableName = "InputFileMessage";
            _connectionStringMain = config.GetConnectionString("Main");
            _connectionStringTest = config.GetConnectionString("Test");
        }

        public void Initialize() => TestDatabaseTableCreationIfNotCreated(_connectionStringTest, _connectionStringMain);

        private void TestDatabaseTableCreationIfNotCreated(string testDatabaseConnectionString, string mainDatabaseConnectionString)
        {
            using (var testConnection = new SqlConnection(_connectionStringTest))
            {
                testConnection.Open();

                string testInputTableCreationQuery = 
                    $"IF OBJECT_ID('{_testInputTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_testInputTableName} " +
                        "(InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, " +
                        "Name NVARCHAR(100) NOT NULL, " +
                        "Extension VARCHAR(10) NOT NULL, " +
                        "Bytes VARBINARY(MAX) NOT NULL, " +
                        "CreationTime DATETIME NOT NULL, " +
                        "LastUpdateTime DATETIME NOT NULL, " +
                        "LastUpdateUser NVARCHAR(100) NOT NULL, " +
                        "StatusId INT NOT NULL);";

                string testInputTableMessageCreationQuery = 
                    $"IF OBJECT_ID('{_testInputMessageTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_testInputMessageTableName} " +
                        "(InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, " +
                        "InputFileId BIGINT NOT NULL REFERENCES InputFile(InputFileId), " +
                        "CreationDate DATETIME NOT NULL, " +
                        "LastUpdateDate DATETIME NOT NULL, " +
                        "LastUpdateUser NVARCHAR(100) NOT NULL, " +
                        "Message NVARCHAR(MAX) NOT NULL);";

                testConnection.Execute(testInputTableCreationQuery);
                testConnection.Execute(testInputTableMessageCreationQuery);
            }

            using (var mainConnection = new SqlConnection(mainDatabaseConnectionString))
            {
                mainConnection.Open();

                string mainInputTableCreationQuery =
                    $"IF OBJECT_ID('{_mainInputTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_mainInputTableName} " +
                        "(InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, " +
                        "Name NVARCHAR(100) NOT NULL, " +
                        "Extension VARCHAR(10) NOT NULL, " +
                        "Bytes VARBINARY(MAX) NOT NULL, " +
                        "CreationTime DATETIME NOT NULL, " +
                        "LastUpdateTime DATETIME NOT NULL, " +
                        "LastUpdateUser NVARCHAR(100) NOT NULL, " +
                        "StatusId INT NOT NULL);";

                string mainInputTableMessageCreationQuery = 
                    $"IF OBJECT_ID('{_mainInputMessageTableName}', 'U') IS NULL " +
                    $"CREATE TABLE {_mainInputMessageTableName} " +
                        "(InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, " +
                        "InputFileId BIGINT NOT NULL REFERENCES InputFile(InputFileId), " +
                        "CreationDate DATETIME NOT NULL, " +
                        "LastUpdateDate DATETIME NOT NULL, " +
                        "LastUpdateUser NVARCHAR(100) NOT NULL, " +
                        "Message NVARCHAR(MAX) NOT NULL);";

                mainConnection.Execute(mainInputTableCreationQuery);
                mainConnection.Execute(mainInputTableMessageCreationQuery);
            }
        }
    }
}
