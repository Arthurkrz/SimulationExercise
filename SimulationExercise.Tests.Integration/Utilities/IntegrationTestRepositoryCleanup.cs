using Microsoft.Extensions.Configuration;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Infrastructure;

namespace SimulationExercise.Tests.Integration.Utilities
{
    public class IntegrationTestRepositoryCleanup
    {
        private readonly string _connectionString;
        private readonly IContextFactory _contextFactory;

        public IntegrationTestRepositoryCleanup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);
        }

        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameOutputFile = "OutputFile";

        private readonly string _tableNameInputFileMessage = "InputFileMessage";
        private readonly string _tableNameReadingMessage = "ReadingMessage";
        private readonly string _tableNameConsistentReadingMessage = "ConsistentReadingMessage";
        private readonly string _tableNameOutputFileMessage = "OutputFileMessage";

        public void Cleanup()
        {
            using (IContext cleanupContext = _contextFactory.Create())
            {
                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameOutputFileMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameOutputFileMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameOutputFile}', 'U') 
                                        IS NOT NULL DELETE FROM {_tableNameOutputFile};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameOutputFile}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameOutputFile}', 
                                        RESEED, 0);");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameConsistentReadingMessage}', 'U')
                                        IS NOT NULL TRUNCATE TABLE {_tableNameConsistentReadingMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameConsistentReading}', 'U')
                                        IS NOT NULL DELETE FROM {_tableNameConsistentReading};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameConsistentReading}', 'U')
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameConsistentReading}', 
                                        RESEED, 0);");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameReadingMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameReadingMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameReading}', 'U')
                                        IS NOT NULL DELETE FROM {_tableNameReading};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameReading}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameReading}', 
                                        RESEED, 0);");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameInputFileMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameInputFileMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameInputFile}', 'U') 
                                        IS NOT NULL DELETE FROM {_tableNameInputFile};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameInputFile}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameInputFile}', 
                                        RESEED, 0);");
                cleanupContext.Commit();
            }
        }
    }
}
