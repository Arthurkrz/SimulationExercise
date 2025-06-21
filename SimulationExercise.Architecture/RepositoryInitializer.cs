using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Architecture
{
    public class RepositoryInitializer : IRepositoryInitializer
    {
        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameInputFileMessage = "InputFileMessage";
        private readonly string _inputFilePrimaryKey = "InputFileId";

        public void Initialize(IContext context) => TestDatabaseTableCreationIfNotCreated(context);

        private void TestDatabaseTableCreationIfNotCreated(IContext context)
        {
            string inputFileTableCreationQuery =
                $"IF OBJECT_ID('{_tableNameInputFile}', 'U') IS NULL " +
                $"CREATE TABLE {_tableNameInputFile} " +
                    "(InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, " +
                    "Name NVARCHAR(100) NOT NULL, " +
                    "Extension VARCHAR(10) NOT NULL, " +
                    "Bytes VARBINARY(MAX) NOT NULL, " +
                    "CreationTime DATETIME NOT NULL, " +
                    "LastUpdateTime DATETIME NOT NULL, " +
                    "LastUpdateUser NVARCHAR(100) NOT NULL, " +
                    "StatusId INT NOT NULL);";

            context.Execute(inputFileTableCreationQuery);

            string inputTableMessageCreationQuery = 
                $"IF OBJECT_ID('{_tableNameInputFileMessage}', 'U') IS NULL " +
                $"CREATE TABLE {_tableNameInputFileMessage} " +
                    "(InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, " +
                    $"InputFileId BIGINT NOT NULL REFERENCES {_tableNameInputFile}({_inputFilePrimaryKey}), " +
                    "CreationDate DATETIME NOT NULL, " +
                    "LastUpdateDate DATETIME NOT NULL, " +
                    "LastUpdateUser NVARCHAR(100) NOT NULL, " +
                    "Message NVARCHAR(MAX) NOT NULL);";

            context.Execute(inputTableMessageCreationQuery);
        }
    }
}
