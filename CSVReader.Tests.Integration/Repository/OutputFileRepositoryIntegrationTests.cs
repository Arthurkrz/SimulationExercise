using Microsoft.Extensions.Configuration;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Enum;
using CSVReader.Core.Utilities;
using CSVReader.Infrastructure;
using CSVReader.Infrastructure.Repository;
using CSVReader.Tests.Integration.Utilities;

namespace CSVReader.Tests.Integration.Repository
{
    public class OutputFileRepositoryIntegrationTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IOutputFileRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<OutputFileInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameOutputFile = "OutputFile";

        public OutputFileRepositoryIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<OutputFileInsertDTO>();

            _contextFactory = new DapperContextFactory(config);

            _repositoryInitializer = new RepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());

            _sut = new OutputFileRepository();
        }

        [Fact]
        public async Task Insert_SuccesfullyInserts_WhenCommitedAsync()
        {
            // Arrange
            await _testRepositoryCleanup.CleanupAsync();

            var currentTime = new DateTime(2025, 05, 12);
            var currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;

            var dto = new OutputFileInsertDTO("filename1", 
                                              new byte[] { 1, 2, 3 }, 
                                              "ext", "ConsistentReading", false);

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
                    ($@"SELECT NAME, EXTENSION, BYTES, CREATIONTIME, 
                        LASTUPDATETIME, LASTUPDATEUSER 
                            FROM {_tableNameOutputFile};");

                Assert.Single(items);
                var retrievedItem = items[0];
                Assert.Equal(dto.Name, retrievedItem.NAME);
                Assert.Equal(dto.Extension, retrievedItem.EXTENSION);
                Assert.True(dto.Bytes.SequenceEqual((byte[])retrievedItem.BYTES));
                Assert.Equal(currentTime, retrievedItem.CREATIONTIME);
                Assert.Equal(currentTime, retrievedItem.LASTUPDATETIME);
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
                // Act
                var results = await _sut.GetByIsExportedAsync(true, "ConsistentReading", context);

                // Assert
                Assert.Equal(2, results.Count);
            }

            // Teardown
            await _testRepositoryCleanup.CleanupAsync();
        }
    }
}
