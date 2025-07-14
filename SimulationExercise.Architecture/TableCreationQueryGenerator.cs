namespace SimulationExercise.Infrastructure
{
    public class TableCreationQueryGenerator
    {
        public static IList<string> GetSimulationDatabaseQueries()
        {
            IList<string> tableCreationQueries = new List<string>();

            string inputFileSQL =
                @"IF OBJECT_ID('InputFile', 'U') IS NULL
                        CREATE TABLE InputFile 
                        (InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, 
                        Name NVARCHAR(100) NOT NULL, 
                        Extension VARCHAR(10) NOT NULL, 
                        Bytes VARBINARY(MAX) NOT NULL, 
                        CreationTime DATETIME NOT NULL, 
                        LastUpdateTime DATETIME NOT NULL, 
                        LastUpdateUser NVARCHAR(100) NOT NULL, 
                        StatusId INT NOT NULL);";

            string inputFileMessageSQL =
                @"IF OBJECT_ID('InputFileMessage', 'U') IS NULL 
                        CREATE TABLE InputFileMessage(
                        InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, 
                        InputFileId BIGINT NOT NULL REFERENCES InputFile(InputFileId), 
                        CreationDate DATETIME NOT NULL, 
                        LastUpdateDate DATETIME NOT NULL, 
                        LastUpdateUser NVARCHAR(100) NOT NULL, 
                        Message NVARCHAR(MAX) NOT NULL);";

            string readingSQL =
                @"IF OBJECT_ID('Reading', 'U') IS NULL
                        CREATE TABLE Reading(
                        ReadingId BIGINT IDENTITY(1,1) PRIMARY KEY,
                        InputFileId BIGINT NOT NULL REFERENCES dbo.InputFile(InputFileId),
                        SensorId BIGINT NOT NULL, 
                        SensorTypeName NVARCHAR(100) NOT NULL,
                        Unit NVARCHAR(10) NOT NULL,
                        StationId BIGINT NOT NULL, 
                        StationName NVARCHAR(100) NOT NULL, 
                        Value INT NOT NULL,
                        Province NVARCHAR(100) NOT NULL,
                        City NVARCHAR(100) NOT NULL,
                        IsHistoric BIT NOT NULL, 
                        StartDate DATETIME NOT NULL, 
                        StopDate DATETIME, 
                        UtmNord INT NOT NULL, 
                        UtmEst INT NOT NULL, 
                        Latitude NVARCHAR(100) NOT NULL, 
                        Longitude NVARCHAR(100) NOT NULL, 
                        CreationTime DATETIME NOT NULL, 
                        LastUpdateTime DATETIME NOT NULL, 
                        LastUpdateUser NVARCHAR(100) NOT NULL, 
                        StatusId INT NOT NULL);";

            string readingMessageSQL =
                @"IF OBJECT_ID('ReadingMessage', 'U') IS NULL
                        CREATE TABLE ReadingMessage(
                        ReadingMessageId BIGINT IDENTITY(1,1) PRIMARY KEY,
                        ReadingId BIGINT NOT NULL REFERENCES dbo.Reading(ReadingId),
                        CreationDate DATETIME NOT NULL,
                        LastUpdateDate DATETIME NOT NULL,
                        LastUpdateUser NVARCHAR(100) NOT NULL,
                        Message NVARCHAR(MAX) NOT NULL);";

            string consistentReadingSQL =
                @"IF OBJECT_ID('ConsistentReading', 'U') IS NULL 
                        CREATE TABLE ConsistentReading(
                        ConsistentReadingId BIGINT IDENTITY(1,1) PRIMARY KEY, 
                        ReadingId BIGINT NOT NULL REFERENCES dbo.Reading(ReadingId), 
                        SensorId BIGINT NOT NULL, 
                        SensorTypeName NVARCHAR(100) NOT NULL, 
                        Unit INT NOT NULL, 
                        Value INT NOT NULL, 
                        Province NVARCHAR(100) NOT NULL, 
                        City NVARCHAR(100) NOT NULL, 
                        IsHistoric BIT NOT NULL, 
                        DaysOfMeasure INT NOT NULL, 
                        UtmNord INT NOT NULL, 
                        UtmEst INT NOT NULL, 
                        Latitude NVARCHAR(100) NOT NULL, 
                        Longitude NVARCHAR(100) NOT NULL, 
                        CreationTime DATETIME NOT NULL, 
                        LastUpdateTime DATETIME NOT NULL, 
                        LastUpdateUser NVARCHAR(100) NOT NULL, 
                        StatusId INT NOT NULL);";

            string consistentReadingMessageSQL =
                @"IF OBJECT_ID('ConsistentReadingMessage', 'U') IS NULL
                        CREATE TABLE ConsistentReadingMessage(
                        ConsistentReadingMessageId BIGINT IDENTITY(1,1) PRIMARY KEY,
                        ConsistentReadingId BIGINT NOT NULL REFERENCES dbo.ConsistentReading(ConsistentReadingId),
                        CreationDate DATETIME NOT NULL,
                        LastUpdateDate DATETIME NOT NULL,
                        LastUpdateUser NVARCHAR(100) NOT NULL,
                        Message NVARCHAR(MAX) NOT NULL);";

            string outputFileSQL =
                @"IF OBJECT_ID('OutputFile', 'U') IS NULL
                        CREATE TABLE OutputFile(
                        OutputFileId BIGINT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL,
                        Bytes VARBINARY(MAX) NOT NULL,
                        Extension VARCHAR(10) NOT NULL,
                        CreationTime DATETIME NOT NULL,
                        LastUpdateTime DATETIME NOT NULL,
                        LastUpdateUser NVARCHAR(100) NOT NULL,
                        StatusId INT NOT NULL);";

            string outputFileMessageSQL =
                @"IF OBJECT_ID('OutputFileMessage', 'U') IS NULL
                        CREATE TABLE OutputFileMessage(
                        OutputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY,
                        OutputFileId BIGINT NOT NULL REFERENCES dbo.OutputFile(OutputFileId),
                        CreationDate DATETIME NOT NULL,
                        LastUpdateDate DATETIME NOT NULL,
                        LastUpdateUser NVARCHAR(100) NOT NULL,
                        Message NVARCHAR(MAX) NOT NULL);";

            tableCreationQueries.Add(inputFileSQL);
            tableCreationQueries.Add(inputFileMessageSQL);
            tableCreationQueries.Add(readingSQL);
            tableCreationQueries.Add(readingMessageSQL);
            tableCreationQueries.Add(consistentReadingSQL);
            tableCreationQueries.Add(consistentReadingMessageSQL);
            tableCreationQueries.Add(outputFileSQL);
            tableCreationQueries.Add(outputFileMessageSQL);

            return tableCreationQueries;
        }

        public static IList<string> GetBasisQueries()
        {
            IList<string> tableCreationQueries = new List<string>();

            string mainTableSQL =
                $@"IF OBJECT_ID('BasisData', 'U') IS NULL 
                        CREATE TABLE BasisData 
                        (BASISID BIGINT PRIMARY KEY, 
                        BASISCODE NVARCHAR(50) NOT NULL, 
                        BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

            tableCreationQueries.Add(mainTableSQL);

            return tableCreationQueries;
        }
    }
}
