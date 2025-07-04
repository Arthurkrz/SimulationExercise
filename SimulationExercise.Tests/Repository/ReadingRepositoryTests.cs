using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Architecture.Repository;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Tests.Repository
{
    public class ReadingRepositoryTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IReadingRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly ITestRepositoryCleanup _testRepositoryCleanup;
        private readonly ITestRepositoryObjectInsertion<ReadingInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameReadingMessage = "ReadingMessage";
        private readonly string _connectionString;

        public ReadingRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<ReadingInsertDTO>();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new ReadingRepository();
        }

        [Fact]
        public void Insert_SuccesfullyInserts_WhenCommited()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(1);

            var currentTime = new DateTime(2025, 05, 12);
            var currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;
            
            var dto = new ReadingInsertDTO(1, 1, "SensorTypeName", 
                2, 1, "StationName", 1, "Province", "City", 
                true, DateTime.Now.Date, DateTime.Now.Date, 1, 1, 
                "Latitude", "Longitude", Status.New);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Insert(dto, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                IList<dynamic> items = assertContext.Query<dynamic>
                    ($@"SELECT INPUTFILEID, SENSORID, SENSORTYPENAME,
                        UNIT, STATIONID, STATIONNAME, VALUE, PROVINCE, 
                        CITY, ISHISTORIC, STARTDATE, STOPDATE, UTMNORD, 
                        UTMEST, LATITUDE, LONGITUDE, LASTUPDATETIME, 
                        CREATIONTIME, LASTUPDATEUSER, STATUSID
                            FROM {_tableNameReading};");

                Assert.Single(items);
                var retrievedItem = items[0];

                Assert.Equal(dto.InputFileId, retrievedItem.InputFileId);
                Assert.Equal(dto.SensorId, retrievedItem.SensorId);
                Assert.Equal(dto.SensorTypeName, retrievedItem.SensorTypeName);
                Assert.Equal(dto.Unit, retrievedItem.Unit);
                Assert.Equal(dto.StationId, retrievedItem.StationId);
                Assert.Equal(dto.StationName, retrievedItem.StationName);
                Assert.Equal(dto.IsHistoric, retrievedItem.IsHistoric);
                Assert.Equal(dto.StartDate, retrievedItem.StartDate);
                Assert.Equal(dto.StopDate, retrievedItem.StopDate);
                Assert.Equal(dto.UtmNord, retrievedItem.UtmNord);
                Assert.Equal(dto.UtmEst, retrievedItem.UtmEst);
                Assert.Equal(dto.Latitude, retrievedItem.Latitude);
                Assert.Equal(dto.Longitude, retrievedItem.Longitude);
                Assert.Equal((int)dto.Status, retrievedItem.StatusId);
                Assert.Equal(currentTime, retrievedItem.LastUpdateTime);
                Assert.Equal(currentTime, retrievedItem.CreationTime);
                Assert.Equal(currentUser, retrievedItem.LastUpdateUser);
            }
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WithSuccessStatus()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(1);

            ReadingGetDTO expectedReturn = new ReadingGetDTO
                (1, 1, 1, "SensorTypeName", "mg/m³", 1, 
                 "StationName", 1, "Province", "City", 
                 true, DateTime.Now.Date, DateTime.Now.Date, 1, 1, 
                 "Latitude", "Longitude", Status.Success);

            ReadingUpdateDTO updateDTO = new ReadingUpdateDTO
                (1, Status.Success, new List<string>());

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<ReadingGetDTO>
                    ($@"SELECT READINGID, INPUTFILEID, SENSORID, 
                        SENSORTYPENAME, UNIT, STATIONID, STATIONNAME, 
                        VALUE, PROVINCE, CITY, ISHISTORIC, STARTDATE, 
                        STOPDATE, UTMNORD, UTMEST, LATITUDE, LONGITUDE, 
                        STATUSID AS STATUS FROM {_tableNameReading} 
                            WHERE READINGID = @READINGID;",
                    new { expectedReturn.ReadingId });

                Assert.Single(result);
                result.First().Should().BeEquivalentTo(expectedReturn);
            }
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WithErrorStatusAndMessages()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(1);

            ReadingGetDTO expectedReturn = new ReadingGetDTO
                (1, 1, 1, "SensorTypeName", "mg/m³", 1,
                 "StationName", 1, "Province", "City",
                 true, DateTime.Now.Date, DateTime.Now.Date, 1, 1,
                 "Latitude", "Longitude", Status.Success);

            ReadingUpdateDTO updateDTO = new ReadingUpdateDTO
                (1, Status.Success, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<ReadingGetDTO>
                    ($@"SELECT READINGID, INPUTFILEID, SENSORID, 
                        SENSORTYPENAME, UNIT, STATIONID, STATIONNAME, 
                        VALUE, PROVINCE, CITY, ISHISTORIC, STARTDATE, 
                        STOPDATE, UTMNORD, UTMEST, LATITUDE, LONGITUDE, 
                        STATUSID AS STATUS FROM {_tableNameReading} 
                            WHERE READINGID = @READINGID;",
                    new { expectedReturn.ReadingId });

                IList<dynamic> messageResult = assertContext.Query<dynamic>
                    ($@"SELECT M.READINGID, R.STATUSID AS STATUS, M.MESSAGE 
                            FROM READING R 
                            INNER JOIN READINGMESSAGE M 
                            ON R.READINGID = M.READINGID 
                            WHERE R.READINGID = @READINGID;",
                    new { expectedReturn.ReadingId });

                Assert.Single(result);
                Assert.Single(messageResult);

                var message = messageResult.First();
                Status status = (Status)(int)message.STATUS;

                result.First().Should().BeEquivalentTo(expectedReturn);

                Assert.Equal((long)message.READINGID, updateDTO.ReadingId);
                Assert.Equal(Status.Error, status);
                Assert.Equal((string)message.MESSAGE, updateDTO.Messages.First());
            }
        }

        [Fact]
        public void GetByStatus_SuccesfullyGets()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(2, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act & Assert
                var results = _sut.GetByStatus(Status.Success, context);
                Assert.Equal(2, results.Count);
            }
        }
    }
}
