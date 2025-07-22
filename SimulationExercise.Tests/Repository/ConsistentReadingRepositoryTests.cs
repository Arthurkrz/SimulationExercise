using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Infrastructure;
using SimulationExercise.Infrastructure.Repository;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using SimulationExercise.Tests.Utilities;

namespace SimulationExercise.Tests.Repository
{
    public class ConsistentReadingRepositoryTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IConsistentReadingRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<ConsistentReadingInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameConsistentReadingMessage = "ConsistentReadingMessage";
        private readonly string _connectionString;

        public ConsistentReadingRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<ConsistentReadingInsertDTO>();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new ConsistentReadingRepository();
        }

        [Fact]
        public void Insert_SuccesfullyInserts_WhenCommited()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertMethodTestSetup();

            var currentTime = new DateTime(2025, 05, 12);
            var currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;

            var dto = new ConsistentReadingInsertDTO(1, 1, 
                "SensorTypeName", Unit.mg_m3, 1, "Province", 
                "City", true, 1, 1, 1, "Latitude", 
                "Longitude", Status.New);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Insert(dto, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                IList<dynamic> items = assertContext.Query<dynamic>
                    ($@"SELECT READINGID, SENSORID, SENSORTYPENAME, 
                        UNIT, VALUE, PROVINCE, CITY, ISHISTORIC, 
                        DAYSOFMEASURE, UTMNORD, UTMEST, LATITUDE, 
                        LONGITUDE, LASTUPDATETIME, CREATIONTIME, 
                        LASTUPDATEUSER, STATUSID FROM {_tableNameConsistentReading};");

                Assert.Single(items);
                var retrievedItem = items[0];
                Assert.Equal(dto.ReadingId, retrievedItem.READINGID);
                Assert.Equal(dto.SensorId, retrievedItem.SENSORID);
                Assert.Equal(dto.SensorTypeName, retrievedItem.SENSORTYPENAME);
                Assert.Equal((int)dto.Unit, retrievedItem.UNIT);
                Assert.Equal(dto.Value, retrievedItem.VALUE);
                Assert.Equal(dto.Province, retrievedItem.PROVINCE);
                Assert.Equal(dto.City, retrievedItem.CITY);
                Assert.Equal(dto.IsHistoric, retrievedItem.ISHISTORIC);
                Assert.Equal(dto.DaysOfMeasure, retrievedItem.DAYSOFMEASURE);
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
            _testRepositoryCleanup.Cleanup();
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WithSuccessStatus()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(1);

            ConsistentReadingGetDTO expectedReturn = new ConsistentReadingGetDTO
                (1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province",
                "City", true, 1, 1, 1, "Latitude", "Longitude", Status.Success);

            ConsistentReadingUpdateDTO updateDTO = new ConsistentReadingUpdateDTO
                (1, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<ConsistentReadingGetDTO>
                    ($@"SELECT CONSISTENTREADINGID, READINGID, SENSORID, 
                        SENSORTYPENAME, UNIT, VALUE, PROVINCE, CITY, ISHISTORIC, 
                        DAYSOFMEASURE, UTMNORD, UTMEST, LATITUDE, LONGITUDE, 
                        STATUSID AS STATUS FROM {_tableNameConsistentReading} 
                            WHERE CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                    new { expectedReturn.ConsistentReadingId });

                Assert.Single(result);
                result.First().Should().BeEquivalentTo(expectedReturn);
            }

            // Teardown
            _testRepositoryCleanup.Cleanup();
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WithErrorStatusAndMessages()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(1);

            ConsistentReadingGetDTO expectedReturn = new ConsistentReadingGetDTO
                (1, 1, 1, "SensorTypeName", Unit.mg_m3, 1, "Province", 
                "City", true, 1, 1, 1, "Latitude", "Longitude", Status.Error);

            ConsistentReadingUpdateDTO updateDTO = new ConsistentReadingUpdateDTO
                (1, Status.Error, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<ConsistentReadingGetDTO>
                    ($@"SELECT CONSISTENTREADINGID, READINGID, SENSORID, 
                        SENSORTYPENAME, UNIT, VALUE, PROVINCE, CITY, 
                        ISHISTORIC, DAYSOFMEASURE, UTMNORD, UTMEST, 
                        LATITUDE, LONGITUDE, STATUSID AS STATUS 
                        FROM {_tableNameConsistentReading}
                            WHERE CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                    new { expectedReturn.ConsistentReadingId });

                IList<dynamic> messageResult = assertContext.Query<dynamic>
                    ($@"SELECT M.CONSISTENTREADINGID, C.STATUSID AS STATUS, M.MESSAGE
                            FROM CONSISTENTREADING C
                            INNER JOIN CONSISTENTREADINGMESSAGE M
                            ON C.CONSISTENTREADINGID = M.CONSISTENTREADINGID
                            WHERE C.CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                    new { expectedReturn.ConsistentReadingId });

                Assert.Single(result);
                Assert.Single(messageResult);

                var message = messageResult.First();
                Status status = (Status)(int)message.STATUS;

                result.First().Should().BeEquivalentTo(expectedReturn);
                Assert.Equal((long)message.CONSISTENTREADINGID, updateDTO.ConsistentReadingId);
                Assert.Equal(Status.Error, status);
                Assert.Equal((string)message.MESSAGE, updateDTO.Messages.First());
            }

            // Teardown
            _testRepositoryCleanup.Cleanup();
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

            // Teardown
            _testRepositoryCleanup.Cleanup();
        }
    }
}
