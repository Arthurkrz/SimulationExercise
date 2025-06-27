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
    public class OutputFieRepositoryTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IOutputFileRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly string _tableNameOutputFile = "OutputFile";
        private readonly string _tableNameOutputFileMessage = "OutputFileMessage";
        private readonly string _connectionString;

        public OutputFieRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new OutputFileRepository();
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

            var dto = new OutputFileInsertDTO("filename1", 
                                              new byte[] { 1, 2, 3 }, 
                                              "ext", Status.New);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Insert(dto, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                IList<dynamic> items = assertContext.Query<dynamic>
                    ($@"SELECT NAME, EXTENSION, BYTES, STATUSID 
                        CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER 
                            FROM {_tableNameOutputFile};");

                Assert.Single(items);
                var retrievedItem = items[0];
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

            OutputFileGetDTO expectedReturn = new OutputFileGetDTO
                (1, 1, "OutputFileName0", new byte[] { 1, 2, 3 }, 
                 "Ext0", Status.Success);

            OutputFileUpdateDTO updateDTO = new OutputFileUpdateDTO
                (1, Status.Success, new List<string>());

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<OutputFileGetDTO>
                    ($@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                        FROM {_tableNameOutputFile} WHERE OUTPUTFILEID = @OUTPUTFILEID;",
                    new { expectedReturn.OutputFileId });

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

            OutputFileGetDTO expectedReturn = new OutputFileGetDTO
                (1, 1, "OutputFileName0", new byte[] { 1, 2, 3 },
                 "Ext0", Status.Error);

            OutputFileUpdateDTO updateDTO = new OutputFileUpdateDTO
                (1, Status.Error, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.Update(updateDTO, context);
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = assertContext.Query<OutputFileGetDTO>
                    ($@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                        FROM {_tableNameOutputFile} WHERE OUTPUTFILEID = @OUTPUTFILEID;",
                    new { expectedReturn.OutputFileId });

                IList<dynamic> messageResult = assertContext.Query<dynamic>
                    ($@"SELECT M.OUTPUTFILEID, O.STATUSID AS STATUS, M.MESSAGE 
                            FROM OUTPUTFILE O 
                            INNER JOIN OUTPUTFILEMESSAGE M 
                            ON O.OUTPUTFILEID = M.OUTPUTFILEID 
                            WHERE O.OUTPUTFILEID = @OUTPUTFILEID;",
                    new { expectedReturn.OutputFileId });

                Assert.Single(result);
                Assert.Single(messageResult);

                var message = messageResult.First();
                Status status = (Status)(int)message.STATUS;

                result.First().Should().BeEquivalentTo(expectedReturn);
                Assert.Equal((long)message.OUTPUTFILEID, updateDTO.OutputFileId);
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
                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameOutputFileMessage}', 'U') 
                                          IS NOT NULL TRUNCATE TABLE {_tableNameOutputFileMessage};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameOutputFile}', 'U') 
                                          IS NOT NULL DELETE FROM {_tableNameOutputFile};");

                cleanupContext.Execute($@"IF OBJECT_ID('{_tableNameOutputFile}', 'U') 
                                          IS NOT NULL DBCC CHECKPOINT ('{_tableNameOutputFile}', 
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
                for (int objectNumber = 0; objectNumber <  numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute
                        ($@"INSERT INTO {_tableNameOutputFile}
                            (NAME, BYTES, EXTENSION, CREATIONTIME, 
                            LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES(@NAME, @BYTES, @EXTENSION, @CREATIONTIME, 
                                       @LASTUPDATETIME, @LASTUPDATEUSER, @STATUSID);",
                        new 
                        {
                            Name = $"OutputFileName{objectNumber}",
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
    }
}
