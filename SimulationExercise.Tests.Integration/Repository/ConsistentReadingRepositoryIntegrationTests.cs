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

namespace SimulationExercise.Tests.Repository
{
    public class ConsistentReadingRepositoryIntegrationTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IConsistentReadingRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<ConsistentReadingInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameConsistentReadingMessage = "ConsistentReadingMessage";
        private readonly string _connectionString;

        public ConsistentReadingRepositoryIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<ConsistentReadingInsertDTO>();

            _connectionString = config.GetConnectionString("Default") ?? 
                throw new ArgumentNullException(nameof(_connectionString));

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
                "Longitude", false);

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
            _testRepositoryCleanup.Cleanup();
        }

        [Fact]
        public void GetByIsExported_SuccesfullyGets()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();
            _testRepositoryObjectInsertion.InsertObjects(2, Status.Success, true);

            using (IContext context = _contextFactory.Create())
            {
                // Act & Assert
                var results = _sut.GetByIsExported(true, context);
                Assert.Equal(2, results.Count);
            }

            // Teardown
            _testRepositoryCleanup.Cleanup();
        }
    }
}
