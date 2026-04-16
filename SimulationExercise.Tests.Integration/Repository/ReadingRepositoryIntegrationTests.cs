using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Infrastructure;
using SimulationExercise.Infrastructure.Repository;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using SimulationExercise.Tests.Integration.Utilities;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Contracts.Infrastructure;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class ReadingRepositoryIntegrationTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IReadingRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<ReadingInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameReadingMessage = "ReadingMessage";

        public ReadingRepositoryIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<ReadingInsertDTO>();

            _contextFactory = new DapperContextFactory(config);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new ReadingRepository();
        }

        [Fact]
        public async Task Insert_SuccesfullyInserts_WhenCommitedAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertMethodTestSetupAsync();

            var currentTime = new DateTime(2025, 05, 12);
            var currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;
            
            var dto = new ReadingInsertDTO(1, 1, "SensorTypeName", 
                "mg/m³", 1, "StationName", 1, "Province", "City", 
                true, DateTime.Now.Date, DateTime.Now.Date, 1, 1, 
                "Latitude", "Longitude", Status.New);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                await _sut.InsertAsync(dto, context);
                context.Commit();
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

                Assert.Equal(dto.InputFileId, retrievedItem.INPUTFILEID);
                Assert.Equal(dto.SensorId, retrievedItem.SENSORID);
                Assert.Equal(dto.SensorTypeName, retrievedItem.SENSORTYPENAME);
                Assert.Equal(dto.Unit, retrievedItem.UNIT);
                Assert.Equal(dto.StationId, retrievedItem.STATIONID);
                Assert.Equal(dto.StationName, retrievedItem.STATIONNAME);
                Assert.Equal(dto.IsHistoric, retrievedItem.ISHISTORIC);
                Assert.Equal(dto.StartDate, retrievedItem.STARTDATE);
                Assert.Equal(dto.StopDate, retrievedItem.STOPDATE);
                Assert.Equal(dto.UtmNord, retrievedItem.UTMNORD);
                Assert.Equal(dto.UtmEst, retrievedItem.UTMEST);
                Assert.Equal(dto.Latitude, retrievedItem.LATITUDE);
                Assert.Equal(dto.Longitude, retrievedItem.LONGITUDE);
                Assert.Equal((int)dto.Status, retrievedItem.STATUSID);
                Assert.Equal(currentTime, retrievedItem.LASTUPDATETIME);
                Assert.Equal(currentTime, retrievedItem.CREATIONTIME);
                Assert.Equal(currentUser, retrievedItem.LASTUPDATEUSER);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task Update_SuccesfullyUpdates_WithSuccessStatusAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(1);

            ReadingGetDTO expectedReturn = new ReadingGetDTO
                (1, 1, 1, "SensorTypeName", "mg/m³", 1, 
                 "StationName", 1, "Province", "City", 
                 true, DateTime.Now.Date, DateTime.Now.Date, 1, 1, 
                 "Latitude", "Longitude", Status.Success);

            ReadingUpdateDTO updateDTO = new ReadingUpdateDTO
                (1, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                await _sut.UpdateAsync(updateDTO, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = await assertContext.QueryAsync<ReadingGetDTO>
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

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task Update_SuccesfullyUpdates_WithErrorStatusAndMessagesAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(1);

            ReadingGetDTO expectedReturn = new ReadingGetDTO
                (1, 1, 1, "SensorTypeName", "mg/m³", 1,
                 "StationName", 1, "Province", "City",
                 true, DateTime.Now.Date, DateTime.Now.Date, 1, 1,
                 "Latitude", "Longitude", Status.Error);

            ReadingUpdateDTO updateDTO = new ReadingUpdateDTO
                (1, Status.Error, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                await _sut.UpdateAsync(updateDTO, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = await assertContext.QueryAsync<ReadingGetDTO>
                    ($@"SELECT READINGID, INPUTFILEID, SENSORID, 
                        SENSORTYPENAME, UNIT, STATIONID, STATIONNAME, 
                        VALUE, PROVINCE, CITY, ISHISTORIC, STARTDATE, 
                        STOPDATE, UTMNORD, UTMEST, LATITUDE, LONGITUDE, 
                        STATUSID AS STATUS FROM {_tableNameReading} 
                            WHERE READINGID = @READINGID;",
                    new { expectedReturn.ReadingId });

                IList<dynamic> messageResult = await assertContext.QueryAsync<dynamic>
                    ($@"SELECT M.READINGID, R.STATUSID AS STATUS, M.MESSAGE 
                            FROM READING R 
                            INNER JOIN {_tableNameReadingMessage} M 
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

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task GetByStatus_SuccesfullyGetsAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(2, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                var results = await _sut.GetByStatusAsync(Status.Success, context);

                // Assert
                Assert.Equal(2, results.Count);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }
    }
}
