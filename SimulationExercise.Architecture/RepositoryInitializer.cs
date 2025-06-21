using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Architecture
{
    public class RepositoryInitializer : IRepositoryInitializer
    {
        public void Initialize(IContext context)
        {
            var connectionString = context.GetConnectionString();
            if (connectionString.Contains("Simulation_Database"))
                SimulationDatabaseInitializer(context);
            else BasisInitializer(context);
        }

        private void SimulationDatabaseInitializer(IContext context)
        {
            string tableNameInputFile = "InputFile";
            string tableNameInputFileMessage = "InputFileMessage";
            string inputFilePrimaryKey = "InputFileId";

            string inputFileTableCreationQuery =
                $@"IF OBJECT_ID('{tableNameInputFile}', 'U') IS NULL
                        CREATE TABLE {tableNameInputFile} 
                        (InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, 
                        Name NVARCHAR(100) NOT NULL, 
                        Extension VARCHAR(10) NOT NULL, 
                        Bytes VARBINARY(MAX) NOT NULL, 
                        CreationTime DATETIME NOT NULL, 
                        LastUpdateTime DATETIME NOT NULL, 
                        LastUpdateUser NVARCHAR(100) NOT NULL, 
                        StatusId INT NOT NULL);";

            context.Execute(inputFileTableCreationQuery);

            string inputTableMessageCreationQuery =
                $@"IF OBJECT_ID('{tableNameInputFileMessage}', 'U') IS NULL 
                        CREATE TABLE {tableNameInputFileMessage} 
                        (InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, 
                        InputFileId BIGINT NOT NULL REFERENCES {tableNameInputFile}({inputFilePrimaryKey}), 
                        CreationDate DATETIME NOT NULL, 
                        LastUpdateDate DATETIME NOT NULL, 
                        LastUpdateUser NVARCHAR(100) NOT NULL, 
                        Message NVARCHAR(MAX) NOT NULL);";

            context.Execute(inputTableMessageCreationQuery);

            context.Commit();
        }

        private void BasisInitializer(IContext context)
        {
            string tableName = "BasisData";

            string mainTableCreationQuery =
                $@"IF OBJECT_ID('{tableName}', 'U') IS NULL 
                        CREATE TABLE {tableName} 
                        (BASISID BIGINT PRIMARY KEY, 
                        BASISCODE NVARCHAR(50) NOT NULL, 
                        BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

            context.Execute(mainTableCreationQuery);

            context.Commit();
        }
    }
}
