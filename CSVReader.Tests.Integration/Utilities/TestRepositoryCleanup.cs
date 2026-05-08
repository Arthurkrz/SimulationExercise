using Microsoft.Extensions.Configuration;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Infrastructure;

namespace CSVReader.Tests.Integration.Utilities
{
    public class TestRepositoryCleanup
    {
        private readonly IContextFactory _contextFactory;

        public TestRepositoryCleanup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _contextFactory = new DapperContextFactory(config);
        }

        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameAverageProvinceData = "AverageProvinceData";
        private readonly string _tableNameOutputFile = "OutputFile";

        private readonly string _tableNameInputFileMessage = "InputFileMessage";
        private readonly string _tableNameReadingMessage = "ReadingMessage";
        private readonly string _tableNameConsistentReadingMessage = "ConsistentReadingMessage";
        private readonly string _tableNameAverageProvinceDataMessage = "AverageProvinceDataMessage";
        private readonly string _tableNameOutputFileMessage = "OutputFileMessage";

        public async Task CleanupAsync()
        {
            using (IContext cleanupContext = _contextFactory.Create())
            {
                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameOutputFileMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameOutputFileMessage};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameOutputFile}', 'U') 
                                        IS NOT NULL DELETE FROM {_tableNameOutputFile};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameOutputFile}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameOutputFile}', 
                                        RESEED, 0);");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameAverageProvinceDataMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameAverageProvinceDataMessage};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameAverageProvinceData}', 'U') 
                                        IS NOT NULL DELETE FROM {_tableNameAverageProvinceData};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameAverageProvinceData}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameAverageProvinceData}', 
                                        RESEED, 0);");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameConsistentReadingMessage}', 'U')
                                        IS NOT NULL TRUNCATE TABLE {_tableNameConsistentReadingMessage};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameConsistentReading}', 'U')
                                        IS NOT NULL DELETE FROM {_tableNameConsistentReading};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameConsistentReading}', 'U')
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameConsistentReading}', 
                                        RESEED, 0);");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameReadingMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameReadingMessage};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameReading}', 'U')
                                        IS NOT NULL DELETE FROM {_tableNameReading};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameReading}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameReading}', 
                                        RESEED, 0);");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameInputFileMessage}', 'U') 
                                        IS NOT NULL TRUNCATE TABLE {_tableNameInputFileMessage};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameInputFile}', 'U') 
                                        IS NOT NULL DELETE FROM {_tableNameInputFile};");

                await cleanupContext.ExecuteAsync($@"IF OBJECT_ID('{_tableNameInputFile}', 'U') 
                                        IS NOT NULL DBCC CHECKIDENT ('{_tableNameInputFile}', 
                                        RESEED, 0);");
                cleanupContext.Commit();
            }
        }
    }
}
