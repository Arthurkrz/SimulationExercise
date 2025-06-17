using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Repository
{
    public class RepositoryIntegrationTest
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly string _tableNameInputFile;
        private readonly string _tableNameInputFileMessage;
        private readonly string _connectionString;

        public RepositoryIntegrationTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(GetJsonDirectoryPath())
                .AddJsonFile("appsettings.test.json").Build();

            _connectionString = config.GetConnectionString("Test");
            _contextFactory = new DapperContextFactory(_connectionString);
            _sut = new InputFileRepository();
            _repositoryInitializer = new RepositoryInitializer();
            _tableNameInputFile = "InputFileTest";
            _tableNameInputFileMessage = "InputFileMessageTest";
        }

        [Fact]
        public void Insert_SuccesfullyInserts_WhenCommited()
        {
            // Arrange
            TestDataCleanup();
            _repositoryInitializer.Initialize();

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
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                IList<dynamic> items = context.Query<dynamic>
                    ("SELECT Name, Extension, Bytes, StatusId, " +
                     "CreationTime, LastUpdateTime, LastUpdateUser " +
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
            _repositoryInitializer.Initialize();
            MultipleObjectInsertion(1);

            long inputFileId = 1;
            InputFileUpdateDTO expectedReturn = new InputFileUpdateDTO(
                inputFileId, Status.Success, new List<string> { "UpdatedMessage" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(expectedReturn, context);
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                IList<(int InputFileId, string Message)> result = context.Query<(int InputFileId, string Message)>
                    ($"SELECT InputFileId, Message FROM {_tableNameInputFileMessage} " +
                      "WHERE InputFileId = @inputFileId;", new { inputFileId });

                Assert.Single(result);

                Assert.Equal(inputFileId, result.First().InputFileId);
                Assert.Equal(expectedReturn.Messages.First(), result.First().Message);
            }
        }

        [Fact]
        public void GetByStatus_SuccesfullyGets()
        {
            // Arrange
            TestDataCleanup();
            _repositoryInitializer.Initialize();
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
                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFileMessage}', 'U') " +
                                       $"IS NOT NULL DROP TABLE {_tableNameInputFileMessage}");

                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFile}', 'U') " +
                                       $"IS NOT NULL DROP TABLE {_tableNameInputFile}");

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
                        "(Name, Bytes, Extension, CreationTime, " +
                        "LastUpdateTime, LastUpdateUser, StatusId) " +
                        "VALUES(@Name, @Bytes, @Extension, @creationTime, " +
                        "@lastUpdateTime, @lastUpdateUser, @StatusId)", 
                        new 
                        { 
                            Name = $"InputFileName{objectNumber}", 
                            Bytes = new byte[] { 1, 2, 3 }, 
                            Extension = $"Ext{objectNumber}", 
                            creationTime, 
                            lastUpdateTime, 
                            lastUpdateUser,
                            StatusId = objectStatus
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

        private static string GetJsonDirectoryPath()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directoryInfo != null && !directoryInfo.GetFiles("appsettings.test.json").Any())
                directoryInfo = directoryInfo.Parent;

            return directoryInfo?.FullName ?? throw new FileNotFoundException("Configuration file not found.");
        }
    }
}