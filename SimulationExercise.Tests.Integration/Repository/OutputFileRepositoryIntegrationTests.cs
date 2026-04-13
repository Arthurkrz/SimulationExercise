using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Infrastructure;
using SimulationExercise.Infrastructure.Repository;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using SimulationExercise.Tests.Integration.Utilities;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Contracts.Infrastructure;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class OutputFileRepositoryIntegrationTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IOutputFileRepository _sut;
        private readonly IRepositoryInitializer _repositoryInitializer;
        private readonly TestRepositoryCleanup _testRepositoryCleanup;
        private readonly TestRepositoryObjectInsertion<OutputFileInsertDTO> _testRepositoryObjectInsertion;

        private readonly string _tableNameOutputFile = "OutputFile";
        private readonly string _tableNameOutputFileMessage = "OutputFileMessage";
        private readonly string _connectionString;

        public OutputFileRepositoryIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            _testRepositoryCleanup = new TestRepositoryCleanup();
            _testRepositoryObjectInsertion = new TestRepositoryObjectInsertion<OutputFileInsertDTO>();

            _connectionString = config.GetConnectionString("Default") ?? 
                throw new ArgumentNullException(nameof(_connectionString));

            //_contextFactory = new DapperContextFactory(_connectionString);

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
                                              "ext", "ConsistentReading", false);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                _sut.InsertAsync(dto, context);
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
                //var results = _sut.GetByIsExportedAsync(true, context);
                //Assert.Equal(2, results.Count);
            }

            // Teardown
            _testRepositoryCleanup.Cleanup();
        }
    }
}
