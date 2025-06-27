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
    public class ConsistentReadingRepositoryTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IConsistentReadingRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly string _tableNameConsistentReading = "ConsistentReading";
        private readonly string _tableNameConsistentReadingMessage = "ConsistentReadingMessage";
        private readonly string _connectionString;

        public ConsistentReadingRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

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
            TestDataCleanup();

            var currentTime = new DateTime(2025, 05, 12);
            const string currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;

            var dto = new ConsistentReadingInsertDTO(1, new byte[] { 1, 2, 3 }, 
                                                     Status.New);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Insert(dto, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                IList<dynamic> items = assertContext.Query<dynamic>
                    ($@"SELECT READINGID, BYTES, STATUSID 
                            FROM {_tableNameConsistentReading};");

                Assert.Single(items);
                var retrievedItem = items[0];
                Assert.Equal(dto.ReadingId, retrievedItem.ReadingId);
                Assert.True(dto.Bytes.SequenceEqual((byte[])retrievedItem.Bytes));
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
            TestDataCleanup();
            MultipleObjectInsertion(1);

            ConsistentReadingGetDTO expectedReturn = new ConsistentReadingGetDTO
                (1, 1, new byte[] { 1, 2, 3 }, Status.Success);

            ConsistentReadingUpdateDTO updateDTO = new ConsistentReadingUpdateDTO
                (1, Status.Success, new List<string>());

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<ConsistentReadingGetDTO>
                    ($@"SELECT CONSISTENTREADINGID, READINGID, BYTES, 
                        STATUSID AS STATUS FROM {_tableNameConsistentReading} 
                            WHERE CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                    new { expectedReturn.ConsistentReadingId });

                Assert.Single(result);
                result.First().Should().BeEquivalentTo(expectedReturn);
            }
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WithErrorStatusAndMessages()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            ConsistentReadingGetDTO expectedReturn = new ConsistentReadingGetDTO
                (1, 1, new byte[] { 1, 2, 3 }, Status.Success);

            ConsistentReadingUpdateDTO updateDTO = new ConsistentReadingUpdateDTO
                (1, Status.Success, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<ConsistentReadingGetDTO>
                    ($@"SELECT CONSISTENTREADINGID, READINGID, BYTES, 
                        STATUSID AS STATUS FROM {_tableNameConsistentReading}
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
        }

        [Fact]
        public void GetByStatus_SuccesfullyGets()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(2, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act & Assert
                var results = _sut.GetByStatus(Status.Success, context);
                Assert.Equal(2, results.Count);
            }
        }

        private void TestDataCleanup()
        {
            using (var cleanupContext = _contextFactory.Create())
            {
                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameConsistentReadingMessage}', 'U')
                                          IS NOT NULL TRUNCATE TABLE {_tableNameConsistentReadingMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameConsistentReading}', 'U')
                                          IS NOT NULL DELETE FROM {_tableNameConsistentReading};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameConsistentReading}', 'U')
                                          IS NOT NULL DBCC CHECKPOINT ('{_tableNameConsistentReading}', 
                                          RESEED, 0);");
                cleanupContext.Commit();
            }
        }

        private void MultipleObjectInsertion(int numberOfObjectsToBeInserted, Status objectStatus = Status.New)
        {
            var creationTime = SystemTime.Now();
            var lastUpdateTime = SystemTime.Now();
            var lastUpdateUser = SystemIdentity.CurrentName();

            using (IContext context = _contextFactory.Create())
            {
                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute
                        ($@"INSERT INTO {_tableNameConsistentReading}
                            (READINGID, BYTES, CREATIONTIME, 
                             LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES(@READINGID, @BYTES, @CREATIONTIME, 
                                       @LASTUPDATETIME, @LASTUPDATEUSER, @STATUSID);",
                        new
                        {
                            ReadingId = objectNumber + 1,
                            Bytes = new byte[] { 1, 2, 3 },
                            creationTime,
                            lastUpdateTime,
                            lastUpdateUser,
                            StatusId = objectStatus
                        });
                }

                context.Commit();
            }
        }
    }
}
