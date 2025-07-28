using Microsoft.Extensions.Configuration;
using SimulationExercise.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Tests.Utilities
{
    public class TestRepositoryObjectInsertion<T>
    {
        private readonly string _connectionString;
        private readonly IContextFactory _contextFactory;
        private readonly Type objectType = typeof(T);

        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameOutputFile = "OutputFile";

        public TestRepositoryObjectInsertion()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);
        }

        public void InsertObjects(int numberOfObjectsToBeInserted, Status status = Status.New)
        {
            if (objectType == typeof(InputFileInsertDTO))
                InputFileRepositoryInsert(numberOfObjectsToBeInserted, status);

            if (objectType == typeof(ReadingInsertDTO))
            {
                InputFileRepositoryInsert(numberOfObjectsToBeInserted, status);
                ReadingRepositoryInsert(numberOfObjectsToBeInserted, status);
            }

            if (objectType == typeof(ConsistentReadingInsertDTO))
            {
                InputFileRepositoryInsert(numberOfObjectsToBeInserted, status);
                ReadingRepositoryInsert(numberOfObjectsToBeInserted, status);
                ConsistentReadingRepositoryInsert(numberOfObjectsToBeInserted, status);
            }

            if (objectType == typeof(OutputFileInsertDTO))
                OutputFileRepositoryInsert(numberOfObjectsToBeInserted, status);
        }

        public void InsertMethodTestSetup()
        {
            if (objectType == typeof(ReadingInsertDTO))
                InputFileRepositoryInsert(1, Status.New);

            if (objectType == typeof(ConsistentReadingInsertDTO))
            {
                InputFileRepositoryInsert(1, Status.New);
                ReadingRepositoryInsert(1, Status.New);                
            }
        }

        private void InputFileRepositoryInsert(int numberOfObjectsToBeInserted, Status status)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute
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

        private void ReadingRepositoryInsert(int numberOfObjectsToBeInserted, Status status)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute
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

        private void ConsistentReadingRepositoryInsert(int numberOfObjectsToBeInserted, Status status)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute
                        ($@"INSERT INTO {_tableNameConsistentReading}
                            (READINGID, SENSORID, SENSORTYPENAME, UNIT, VALUE, 
                             PROVINCE, CITY, ISHISTORIC, DAYSOFMEASURE, UTMNORD, 
                             UTMEST, LATITUDE, LONGITUDE, CREATIONTIME, 
                             LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES(@READINGID, @SENSORID, @SENSORTYPENAME, @UNIT, 
                                       @VALUE, @PROVINCE, @CITY, @ISHISTORIC, 
                                       @DAYSOFMEASURE, @UTMNORD, @UTMEST, 
                                       @LATITUDE, @LONGITUDE, @CREATIONTIME, 
                                       @LASTUPDATETIME, @LASTUPDATEUSER, @STATUSID);",
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
                            StatusId = status
                        });
                }

                context.Commit();
            }
        }

        private void OutputFileRepositoryInsert(int numberOfObjectsToBeInserted, Status status)
        {
            using (IContext context = _contextFactory.Create())
            {
                var creationTime = SystemTime.Now();
                var lastUpdateTime = SystemTime.Now();
                var lastUpdateUser = SystemIdentity.CurrentName();

                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute
                        ($@"INSERT INTO {_tableNameOutputFile}
                        (NAME, BYTES, EXTENSION, CREATIONTIME, 
                        LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                            VALUES(@NAME, @BYTES, @EXTENSION, @CREATIONTIME, 
                                    @LASTUPDATETIME, @LASTUPDATEUSER, @STATUSID);",
                        new
                        {
                            Name = $"OutputFileName{objectNumber}",
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
    }
}