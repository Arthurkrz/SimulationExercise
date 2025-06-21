using FluentAssertions;
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
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameInputFileMessage = "InputFileMessage";
        private readonly string _connectionString;

        public RepositoryIntegrationTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(GetJsonDirectoryPath())
                .AddJsonFile("appsettings.test.json").Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new InputFileRepository();
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
        public void Update_SuccesfullyUpdates_WithSuccessStatus()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            InputFileGetDTO expectedReturn = new InputFileGetDTO
                (1, "InputFileName0", new byte[] { 1, 2, 3 },
                    "Ext0", Status.Success);

            InputFileUpdateDTO updateDTO = new InputFileUpdateDTO
                (1, Status.Success, new List<string>());

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            // Assert
            using (IContext assertContext = _contextFactory.Create())
            {
                IList<InputFileGetDTO> result = assertContext.Query<InputFileGetDTO>
                    ("SELECT InputFileId, Name, Extension, Bytes, StatusId AS Status " +
                    $"FROM {_tableNameInputFile} WHERE InputFileId = @InputFileId;", 
                    new { expectedReturn.InputFileId });

                Assert.Single(result);

                Assert.Equal(Status.Success, result.First().Status);
                Assert.Equal(1, result.First().InputFileId);
            }
        }

        [Fact]
        public void Update_SuccesfullyUpdates_WithErrorStatusAndMessages()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            InputFileGetDTO expectedReturn = new InputFileGetDTO
                (1, "InputFileName0", new byte[] { 1, 2, 3 },
                    "Ext0", Status.Error);

            InputFileUpdateDTO updateDTO = new InputFileUpdateDTO
                (1, Status.Error, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                IList<InputFileGetDTO> result = assertContext.Query<InputFileGetDTO>
                    ("SELECT InputFileId, Name, Extension, Bytes, StatusId AS Status" +
                    $"FROM {_tableNameInputFile} WHERE InputFileId = @InputFileId;", 
                    new { expectedReturn.InputFileId });

                IList<InputFileUpdateDTO> messageResult = assertContext.Query<InputFileUpdateDTO>
                    ($"SELECT InputFileId, Message FROM {_tableNameInputFileMessage} " +
                      "WHERE InputFileId = @InputFileId;", new { expectedReturn.InputFileId });

                Assert.Single(result);
                Assert.Single(messageResult);

                result.First().Should().BeEquivalentTo(expectedReturn);
                messageResult.First().Should().BeEquivalentTo(updateDTO);
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
                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFileMessage}', 'U') " +
                       $"IS NOT NULL TRUNCATE TABLE {_tableNameInputFileMessage}");

                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFile}', 'U') " +
                       $"IS NOT NULL DELETE FROM {_tableNameInputFile}");

                cleanupContext.Execute($"IF OBJECT_ID('{_tableNameInputFile}', 'U') " +
                       $"IS NOT NULL DBCC CHECKIDENT ('{_tableNameInputFile}', RESEED, 0)");

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