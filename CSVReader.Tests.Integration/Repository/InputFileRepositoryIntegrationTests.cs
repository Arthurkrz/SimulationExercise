using FluentAssertions;
using Microsoft.Extensions.Configuration;
using CSVReader.Infrastructure;
using CSVReader.Infrastructure.Repository;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Enum;
using CSVReader.Core.Utilities;
using CSVReader.Tests.Integration.Utilities;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Contracts.Infrastructure;

namespace CSVReader.Tests.Integration.Repository
{
    public class InputFileRepositoryIntegrationTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<InputFileInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameInputFile = "InputFile";
        private readonly string _tableNameInputFileMessage = "InputFileMessage";

        public InputFileRepositoryIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<InputFileInsertDTO>();

            _contextFactory = new DapperContextFactory(config);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new InputFileRepository();
        }

        [Fact]
        public async Task Insert_SuccesfullyInserts_WhenCommitedAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();

            var currentTime = new DateTime(2025, 05, 12);
            const string currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;

            var dto = new InputFileInsertDTO("InputFileName1", 
                                             new byte[] { 1, 2, 3 },
                                             "ext", Status.New);

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
                    ($@"SELECT NAME, EXTENSION, BYTES, STATUSID, 
                        CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER 
                            FROM {_tableNameInputFile};");

                Assert.Single(items);
                var retrievedItem = items[0];
                Assert.Equal(dto.Name, retrievedItem.NAME);
                Assert.Equal(dto.Extension, retrievedItem.EXTENSION);
                Assert.True(dto.Bytes.SequenceEqual((byte[])retrievedItem.BYTES));
                Assert.Equal((int)dto.Status, retrievedItem.STATUSID);
                Assert.Equal(currentTime, retrievedItem.LASTUPDATETIME);
                Assert.Equal(currentTime, retrievedItem.CREATIONTIME);
                Assert.Equal(currentUser, retrievedItem.LASTUPDATEUSER);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task Update_SuccesfullyUpdates_WithSuccessStatusAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(1, Status.Success);

            InputFileGetDTO expectedReturn = new InputFileGetDTO
                (1, "InputFileName0", new byte[] { 1, 2, 3 },
                 "Ext0", Status.Success);

            InputFileUpdateDTO updateDTO = new InputFileUpdateDTO
                (1, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                await _sut.UpdateAsync(updateDTO, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = await assertContext.QueryAsync<InputFileGetDTO>
                    ($@"SELECT INPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                            FROM {_tableNameInputFile} WHERE INPUTFILEID = @INPUTFILEID;",
                    new { expectedReturn.InputFileId });

                Assert.Single(result);
                result.First().Should().BeEquivalentTo(expectedReturn);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task Update_SuccesfullyUpdates_WithErrorStatusAndMessagesAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(1);

            InputFileGetDTO expectedReturn = new InputFileGetDTO
                (1, "InputFileName0", new byte[] { 1, 2, 3 },
                 "Ext0", Status.Error);

            InputFileUpdateDTO updateDTO = new InputFileUpdateDTO
                (1, Status.Error, new List<string> { "Error0" });

            using (IContext context = _contextFactory.Create())
            {
                // Act
                await _sut.UpdateAsync(updateDTO, context);
                context.Commit();
            }

            using (IContext assertContext = _contextFactory.Create())
            {
                // Assert
                var result = await assertContext.QueryAsync<InputFileGetDTO>
                    ($@"SELECT INPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                            FROM {_tableNameInputFile} WHERE INPUTFILEID = @INPUTFILEID;", 
                    new { expectedReturn.InputFileId });

                IList<dynamic> messageResult = await assertContext.QueryAsync<dynamic>
                    ($@"SELECT M.INPUTFILEID, F.STATUSID AS STATUS, M.MESSAGE
                            FROM INPUTFILE F 
                            INNER JOIN {_tableNameInputFileMessage} M
                            ON F.INPUTFILEID = M.INPUTFILEID
                            WHERE F.INPUTFILEID = @INPUTFILEID;",
                    new { expectedReturn.InputFileId });

                Assert.Single(result);
                Assert.Single(messageResult);

                var message = messageResult.First();
                Status status = (Status)(int)message.STATUS;

                result.First().Should().BeEquivalentTo(expectedReturn);
                Assert.Equal((long)message.INPUTFILEID, updateDTO.InputFileId);
                Assert.Equal(Status.Error, status);
                Assert.Equal((string)message.MESSAGE, updateDTO.Messages.First());
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }

        [Fact]
        public async Task GetByStatus_SuccesfullyGetsAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();
            await _testRepositoryObjectInsertion.InsertObjectsAsync(2, Status.Success);

            using (IContext context = _contextFactory.Create())
            {
                // Act & Assert
                var results = await _sut.GetByStatusAsync(Status.Success, context);
                Assert.Equal(2, results.Count);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }
    }
}