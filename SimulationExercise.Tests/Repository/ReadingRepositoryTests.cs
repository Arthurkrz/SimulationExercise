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
        private readonly string _tableNameReading = "Reading";
        private readonly string _tableNameReadingMessage = "ReadingMessage";
        private readonly string _connectionString;

        public ReadingRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

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
            TestDataCleanup();

            var currentTime = new DateTime(2025, 05, 12);
            const string currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;

            var dto = new ReadingInsertDTO(1, new byte[] { 1, 2, 3 },
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
                    ($@"SELECT INPUTFILEID, BYTES, STATUSID 
                            FROM {_tableNameReading};");

                Assert.Single(items);
                var retrievedItem = items[0];
                Assert.Equal(dto.InputFileId, retrievedItem.InputFileId);
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

            ReadingGetDTO expectedReturn = new ReadingGetDTO
                (1, 1, new byte[] { 1, 2, 3 }, Status.Success);

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
                var result = assertContext.Query<ConsistentReadingGetDTO>
                    ($@"SELECT READINGID, INPUTFILEID, BYTES, 
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
            TestDataCleanup();
            MultipleObjectInsertion(1);

            ReadingGetDTO expectedReturn = new ReadingGetDTO
                (1, 1, new byte[] { 1, 2, 3 }, Status.Success);

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
                    ($@"SELECT READINGID, INPUTFILEID, BYTES, 
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
                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameReadingMessage}', 'U') 
                                          IS NOT NULL TRUNCATE TABLE {_tableNameReadingMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameReading}', 'U')
                                          IS NOT NULL DELETE FROM {_tableNameReading};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameReading}', 'U') 
                                          IS NOT NULL DBCC CHECKPOINT ('{_tableNameReading}', 
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
                        ($@"INSERT INTO {_tableNameReading}
                            (INPUTFILEID, BYTES, CREATIONDATE, 
                             LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES(@INPUTFILEID, @BYTES, @CREATIONTIME,
                                       @LASTUPDATETIME, @LASTUPDATEUSER, @STATUSID);",
                        new
                        {
                            InputFileId = objectNumber + 1,
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
