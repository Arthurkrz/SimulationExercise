using Microsoft.Extensions.Configuration;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Enum;
using CSVReader.Core.Utilities;
using CSVReader.Infrastructure;

namespace CSVReader.Tests.Integration.Utilities
{
    public class TestRepositoryObjectInsertion<T>
    {
        private readonly IContextFactory _contextFactory;
        private readonly Type objectType = typeof(T);

        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameAverageProvinceData = "AverageProvinceData";
        private readonly string _tableNameOutputFile = "OutputFile";

        public TestRepositoryObjectInsertion()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _contextFactory = new DapperContextFactory(config);
        }

        public async Task InsertObjectsAsync(int numberOfObjectsToBeInserted, Status status = Status.New, bool isExported = false)
        {
            if (objectType == typeof(InputFileInsertDTO))
                await InputFileRepositoryInsertAsync(numberOfObjectsToBeInserted, status);

            if (objectType == typeof(ReadingInsertDTO))
            {
                await InputFileRepositoryInsertAsync(numberOfObjectsToBeInserted, status);
                await ReadingRepositoryInsertAsync(numberOfObjectsToBeInserted, status);
            }

            if (objectType == typeof(ConsistentReadingInsertDTO))
            {
                await InputFileRepositoryInsertAsync(numberOfObjectsToBeInserted, status);
                await ReadingRepositoryInsertAsync(numberOfObjectsToBeInserted, status);
                await ConsistentReadingRepositoryInsertAsync(numberOfObjectsToBeInserted, isExported);
            }

            if (objectType == typeof(AverageProvinceDataInsertDTO))
            {
                await InputFileRepositoryInsertAsync(numberOfObjectsToBeInserted, status);
                await ReadingRepositoryInsertAsync(numberOfObjectsToBeInserted, status);
                await ConsistentReadingRepositoryInsertAsync(numberOfObjectsToBeInserted, isExported);
                await AverageProvinceDataRepositoryInsertAsync(numberOfObjectsToBeInserted, isExported);
            }

            if (objectType == typeof(OutputFileInsertDTO))
                await OutputFileRepositoryInsertAsync(numberOfObjectsToBeInserted, isExported);
        }

        public async Task InsertMethodTestSetupAsync()
        {
            if (objectType == typeof(ReadingInsertDTO))
                await InputFileRepositoryInsertAsync(1, Status.New);

            if (objectType == typeof(ConsistentReadingInsertDTO))
            {
                await InputFileRepositoryInsertAsync(1, Status.New);
                await ReadingRepositoryInsertAsync(1, Status.New);
            }
        }

        private async Task InputFileRepositoryInsertAsync(int numberOfObjectsToBeInserted, Status status)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    await context.ExecuteAsync
                        ($@"INSERT INTO {_tableNameInputFile} 
                        (NAME, BYTES, EXTENSION, CREATIONTIME,
                        LASTUPDATETIME, LASTUPDATEUSER, STATUSID) 
                            VALUES(@NAME, @BYTES, @EXTENSION, @CREATIONTIME, 
                                    @LASTUPDATETIME, @LASTUPDATEUSER, @STATUSID);",
                        new
                        {
                            Name = $"InputFileName{objectNumber}",
                            Bytes = new byte[] { 1, 2, 3 },
                            Extension = $"Ext{objectNumber}",
                            creationTime,
                            lastUpdateTime,
                            lastUpdateUser,
                            StatusId = status
                        });
                }

                context.Commit();
            }
        }

        private async Task ReadingRepositoryInsertAsync(int numberOfObjectsToBeInserted, Status status)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    await context.ExecuteAsync
                        ($@"INSERT INTO {_tableNameReading}
                        (INPUTFILEID, SENSORID, SENSORTYPENAME, 
                            UNIT, STATIONID, STATIONNAME, VALUE, 
                            PROVINCE, CITY, ISHISTORIC, STARTDATE, 
                            STOPDATE, UTMNORD, UTMEST, LATITUDE, 
                            LONGITUDE, CREATIONTIME, LASTUPDATETIME, 
                            LASTUPDATEUSER, STATUSID)
                            VALUES(@INPUTFILEID, @SENSORID, @SENSORTYPENAME, @UNIT,  
                                    @STATIONID, @STATIONNAME, @VALUE, @PROVINCE, 
                                    @CITY, @ISHISTORIC, @STARTDATE, @STOPDATE, 
                                    @UTMNORD, @UTMEST, @LATITUDE, @LONGITUDE, 
                                    @CREATIONTIME, @LASTUPDATETIME, 
                                    @LASTUPDATEUSER, @STATUSID);",
                        new
                        {
                            InputFileId = objectNumber + 1,
                            SensorId = objectNumber + 1,
                            SensorTypeName = "SensorTypeName",
                            Unit = "mg/m³",
                            StationId = objectNumber + 1,
                            StationName = "StationName",
                            Value = objectNumber + 1,
                            Province = "Province",
                            City = "City",
                            IsHistoric = true,
                            StartDate = DateTime.Now.Date,
                            StopDate = DateTime.Now.Date,
                            UtmNord = objectNumber + 1,
                            UtmEst = objectNumber + 1,
                            Latitude = "Latitude",
                            Longitude = "Longitude",
                            creationTime,
                            lastUpdateTime,
                            lastUpdateUser,
                            StatusId = status
                        });
                }

                context.Commit();
            }
        }

        private async Task ConsistentReadingRepositoryInsertAsync(int numberOfObjectsToBeInserted, bool isExported)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    await context.ExecuteAsync
                        ($@"INSERT INTO {_tableNameConsistentReading}
                            (READINGID, SENSORID, SENSORTYPENAME, UNIT, VALUE, 
                             PROVINCE, CITY, ISHISTORIC, DAYSOFMEASURE, UTMNORD, 
                             UTMEST, LATITUDE, LONGITUDE, CREATIONTIME, 
                             LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED)
                                VALUES(@READINGID, @SENSORID, @SENSORTYPENAME, @UNIT, 
                                       @VALUE, @PROVINCE, @CITY, @ISHISTORIC, 
                                       @DAYSOFMEASURE, @UTMNORD, @UTMEST, 
                                       @LATITUDE, @LONGITUDE, @CREATIONTIME, 
                                       @LASTUPDATETIME, @LASTUPDATEUSER, 
                                       @ISEXPORTED);",
                        new
                        {
                            ReadingId = objectNumber + 1,
                            SensorId = objectNumber + 1,
                            SensorTypeName = "SensorTypeName",
                            Unit = Unit.mg_m3,
                            Value = objectNumber + 1,
                            Province = "Province",
                            City = "City",
                            IsHistoric = true,
                            DaysOfMeasure = objectNumber + 1,
                            UtmNord = objectNumber + 1,
                            UtmEst = objectNumber + 1,
                            Latitude = "Latitude",
                            Longitude = "Longitude",
                            creationTime,
                            lastUpdateTime,
                            lastUpdateUser,
                            IsExported = isExported
                        });
                }

                context.Commit();
            }
        }

        private async Task AverageProvinceDataRepositoryInsertAsync(int numberOfObjectsToBeInserted, bool isExported)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now;
                var lastUpdateUser = SystemTime.Now;

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    await context.ExecuteAsync
                        ($@"INSERT INTO {_tableNameAverageProvinceData} 
                        (PROVNCE, SENSORTYPENAME, AVERAGEVALUE, 
                        UNIT, AVERAGEDAYSOFMEASURE, ISEXPORTED) 
                            VALUES(@PROVINCE, @SENSORTYPENAME, 
                                   @AVERAGEVALUE, @UNIT, 
                                   @AVERAGEDAYSOFMEASE, 
                                   @ISEXPORTED);",
                        new
                        {
                            Province = $"Province{objectNumber}",
                            SensorTypeName = $"SensorTypeName{objectNumber}",
                            AverageValue = objectNumber,
                            Unit = Unit.mg_m3,
                            AverageDaysOfMeasure = objectNumber,
                            IsExported = isExported
                        });
                }

                context.Commit();
            }
        }

        private async Task OutputFileRepositoryInsertAsync(int numberOfObjectsToBeInserted, bool isExported)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    await context.ExecuteAsync
                        ($@"INSERT INTO {_tableNameOutputFile}
                        (NAME, BYTES, EXTENSION, OBJECTTYPE, CREATIONTIME, 
                        LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED)
                            VALUES(@NAME, @BYTES, @EXTENSION, @OBJECTTYPE, 
                                   @CREATIONTIME, @LASTUPDATETIME, 
                                   @LASTUPDATEUSER, @ISEXPORTED);",
                        new
                        {
                            Name = $"OutputFileName{objectNumber}",
                            Bytes = new byte[] { 1, 2, 3 },
                            Extension = $"Ext{objectNumber}",
                            ObjectType = "ConsistentReading",
                            creationTime,
                            lastUpdateTime,
                            lastUpdateUser,
                            IsExported = isExported,
                        });
                }

                context.Commit();
            }
        }
    }
}
