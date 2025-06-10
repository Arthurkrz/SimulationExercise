using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Tests.Repository
{
    public class RepositoryIntegrationTest
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _sut;
        private readonly string _tableNameInputFile;
        private readonly string _tableNameInputFileMessage;

        public RepositoryIntegrationTest(IContextFactory contextFactory, IInputFileRepository inputFileRepository)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json").Build();

            _contextFactory = contextFactory ?? throw new 
                ArgumentNullException(nameof(contextFactory));

            _sut = new InputFileRepository();
            _tableNameInputFile = "InputFile";
            _tableNameInputFileMessage = "InputFileMessage";
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

            var dto = new InputFileInsertDTO("filename1", 
                                             new byte[] { 1, 2, 3 },
                                             "ext", Status.New);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Insert(dto, context);
                context.Commit();
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                IList<dynamic> items = context.Query<dynamic>
                    ("SELECT Name, Extension, Bytes, StatusId, " +
                     "CreationTime, LastUpdate, LastUpdateUser " +
                    $"FROM {_tableNameInputFile}");

                var retrievedItem = items[0];
                Assert.Equal(1, items.Count);
                Assert.Equal(dto.Name, retrievedItem.Name);
                Assert.Equal(dto.Extension, retrievedItem.Extension);
                Assert.True(dto.Bytes.SequenceEqual((byte[])retrievedItem.Bytes));
                Assert.Equal((int)dto.Status, retrievedItem.StatusId);
                Assert.Equal(currentTime, retrievedItem.LastUpdateTime);
                Assert.Equal(currentTime, retrievedItem.CreationTime);
                Assert.Equal(currentUser, retrievedItem.LastUpdateUser);
            }
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WhenCommited()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            int inputFileId = 2;
            InputFileUpdateDTO expectedReturn = new InputFileUpdateDTO(
                inputFileId, Status.Success, new List<string> { "UpdatedMessage" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(expectedReturn, context);
                context.Commit();
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                IList<InputFileUpdateDTO> results = context.Query<InputFileUpdateDTO>
                    ($"SELECT Message FROM {_tableNameInputFileMessage} " +
                     $"WHERE InputFileId = @inputFileId;");

                Assert.Single(results);

                Assert.Equal(inputFileId, results.First().InputFileId);
                Assert.Equal(expectedReturn.Status, results.First().Status);
                Assert.Equal(expectedReturn.Messages, results.First().Messages);
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
                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFile}', 'U') " +
                                       $"IS NOT NULL DROP TABLE {_tableNameInputFile}");

                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFileMessage}', 'U') " +
                                       $"IS NOT NULL DROP TABLE {_tableNameInputFileMessage}");

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
                    context.Execute($"INSERT INTO {_tableNameInputFile}" +
                        "(Name, Bytes, Extension, StatusId, CreationTime, " +
                        "LastUpdateTime, LastUpdateUser) " +
                        "VALUES(@name, @bytes, @extension, @creationTime, " +
                        "@lastUpdateTime, @lastUpdateUser, @statusId)", 
                        new 
                        { 
                            name = $"InputFileName{objectNumber}", 
                            bytes = new byte[] { 1, 2, 3 }, 
                            extension = $"InputFileExtension{objectNumber}", 
                            statusId = objectStatus, 
                            creationTime, 
                            lastUpdateTime, 
                            lastUpdateUser
                        });

                    context.Execute($"INSERT INTO {_tableNameInputFileMessage}" +
                        "(InputFileId, CreationDate, LastUpdateDate, LastUpdateUser, Message) " +
                        "VALUES(@inputFileId, @creationDate, @lastUpdateDate, @lastUpdateUser, @message)",
                        new
                        {
                            inputFileId = 1,
                            creationDate = creationTime,
                            lastUpdateDate = lastUpdateTime,
                            lastUpdateUser,
                            message = new List<string> { $"Message{objectNumber}" }
                        });
                }

                context.Commit();
            }
        }
    }
}