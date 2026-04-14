using Microsoft.Extensions.Configuration;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using SimulationExercise.Infrastructure;
using SimulationExercise.Infrastructure.Repository;
using SimulationExercise.Tests.Integration.Utilities;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class ConsistentReadingRepositoryIntegrationTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IConsistentReadingRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<ConsistentReadingInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameConsistentReading = "ConsistentReading";

        public ConsistentReadingRepositoryIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<ConsistentReadingInsertDTO>();

            _contextFactory = new DapperContextFactory(config);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new ConsistentReadingRepository();
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

            var dto = new ConsistentReadingInsertDTO(1, 1, 
                "SensorTypeName", Unit.mg_m3, 1, "Province", 
                "City", true, 1, 1, 1, "Latitude", 
                "Longitude", false);

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
                    ($@"SELECT READINGID, SENSORID, SENSORTYPENAME, 
                        UNIT, VALUE, PROVINCE, CITY, ISHISTORIC, 
                        DAYSOFMEASURE, UTMNORD, UTMEST, LATITUDE, 
                        LONGITUDE, LASTUPDATETIME, CREATIONTIME, 
                        LASTUPDATEUSER, ISEXPORTED, STATUSID FROM 
                        {_tableNameConsistentReading};");

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
                Assert.Equal(dto.IsExported, retrievedItem.ISEXPORTED);
                Assert.Equal(currentTime, retrievedItem.LASTUPDATETIME);
                Assert.Equal(currentTime, retrievedItem.CREATIONTIME);
                Assert.Equal(currentUser, retrievedItem.LASTUPDATEUSER);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task GetByIsExported_SuccesfullyGetsAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(2, Status.Success, true);

            using (IContext context = _contextFactory.Create())
            {
                // Act & Assert
                var results = await _sut.GetByIsExportedAsync(true, context);
                Assert.Equal(2, results.Count);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }
    }
}
