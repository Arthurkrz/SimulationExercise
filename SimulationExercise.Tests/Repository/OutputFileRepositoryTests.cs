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
    public class OutputFileRepositoryTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IOutputFileRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<OutputFileInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameOutputFile = "OutputFile";
        private readonly string _tableNameOutputFileMessage = "OutputFileMessage";
        private readonly string _connectionString;

        public OutputFileRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<OutputFileInsertDTO>();

            _connectionString = config.GetConnectionString("Default") ?? 
                throw new ArgumentNullException(nameof(_connectionString));

            _contextFactory = new DapperContextFactory(_connectionString);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new OutputFileRepository();
        }

        [Fact]
        public void Insert_SuccesfullyInserts_WhenCommited()
        {
            // Arrange
            _testRepositoryCleanup.Cleanup();

            var currentTime = new DateTime(2025, 05, 12);
            var currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;

            var dto = new OutputFileInsertDTO("filename1", 
                                              new byte[] { 1, 2, 3 }, 
                                              "ext", Status.New);

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
                    ($@"SELECT NAME, EXTENSION, BYTES, STATUSID, 
                        CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER 
                            FROM {_tableNameOutputFile};");

                Assert.Single(items);
                var retrievedItem = items[0];
                Assert.Equal(dto.Name, retrievedItem.NAME);
                Assert.Equal(dto.Extension, retrievedItem.EXTENSION);
                Assert.True(dto.Bytes.SequenceEqual((byte[])retrievedItem.BYTES));
                Assert.Equal((int)dto.Status, retrievedItem.STATUSID);
                Assert.Equal(currentTime, retrievedItem.CREATIONTIME);
                Assert.Equal(currentTime, retrievedItem.LASTUPDATETIME);
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

            OutputFileGetDTO expectedReturn = new OutputFileGetDTO
                (1, "OutputFileName0", new byte[] { 1, 2, 3 }, 
                 "Ext0", Status.Success);

            OutputFileUpdateDTO updateDTO = new OutputFileUpdateDTO
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
                var result = assertContext.Query<OutputFileGetDTO>
                    ($@"SELECT OUTPUTFILEID, NAME, 
                        BYTES, EXTENSION, STATUSID AS STATUS 
                            FROM {_tableNameOutputFile} 
                            WHERE OUTPUTFILEID = @OUTPUTFILEID;",
                    new { expectedReturn.OutputFileId });

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

            OutputFileGetDTO expectedReturn = new OutputFileGetDTO
                (1, "OutputFileName0", new byte[] { 1, 2, 3 },
                 "Ext0", Status.Error);

            OutputFileUpdateDTO updateDTO = new OutputFileUpdateDTO
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
                var result = assertContext.Query<OutputFileGetDTO>
                    ($@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                        FROM {_tableNameOutputFile} WHERE OUTPUTFILEID = @OUTPUTFILEID;",
                    new { expectedReturn.OutputFileId });

                IList<dynamic> messageResult = assertContext.Query<dynamic>
                    ($@"SELECT M.OUTPUTFILEID, O.STATUSID AS STATUS, M.MESSAGE 
                            FROM OUTPUTFILE O 
                            INNER JOIN {_tableNameOutputFileMessage} M 
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
