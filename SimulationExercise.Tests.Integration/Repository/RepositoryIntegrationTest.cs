using Microsoft.Extensions.Configuration;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryIntegrationTest
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _inputFileRepository;

        public RepositoryIntegrationTest(IContextFactory contextFactory, IInputFileRepository inputFileRepository)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyTestDataBase"]
                                                          .ConnectionString;

            _contextFactory = contextFactory ?? throw new 
                ArgumentNullException(nameof(contextFactory));

            _inputFileRepository = new InputFileRepository;
        }

        public void TestCleanup()
        {
            using (IContext context = _contextFactory.Create())
            {
                context.Execute("DELETE FROM InputFileMessage");
                context.Execute("DELETE FROM InputFile");
                context.Commit();
            }
        }

        [Fact]
        public void Insert_FullInsertDTO_Commited()
        {
            // Arrange
            var dto = new InputFileInsertDTO("filename1", new byte[] { 1, 2, 3 }, 
                                             "ext", Status.New);

            var currentTime = new DateTime(2025, 05, 12);
            const string currentUser = "currentUser1";
            SystemTime.Now = () => currentTime;
            SystemIdentity.CurrentName = () => currentUser;
            using (IContext context = _contextFactory.Create())
            {
                // Act
                _inputFileRepository.Insert(dto, context);
                context.Commit();
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                IList<dynamic> items = context.Query<dynamic>
                    ("SELECT Name, Extension, Bytes, StatusId, " +
                    "CreationTime, LastUpdate, LastUpdateUser " +
                    "FROM InputFile");

                Assert.Equal(1, items.Count);
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
    }
}