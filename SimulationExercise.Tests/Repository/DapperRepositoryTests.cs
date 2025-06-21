using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Repository
{
    public class DapperRepositoryTests
    {
        private readonly string _tableName;
        private readonly string _connectionString;
        private IContextFactory _contextFactory;
        private IRepositoryInitializer _repositoryInitializer;

        public DapperRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(GetJsonDirectoryPath())
                .AddJsonFile("appsettings.basistest.json").Build();

            _connectionString = config.GetConnectionString("DefaultDatabase");
            _contextFactory = new DapperContextFactory(_connectionString);

            _repositoryInitializer = new DapperRepositoryInitializer();
            _repositoryInitializer.Initialize(_contextFactory.Create());
            _tableName = "BasisData";
        }

        [Fact]
        public void Query_ReturnsExpectedDynamicItem()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            const long basisId = -1;
            var basis = new Basis(-1, "BasisCode", "BasisDescription");

            using (IContext context = _contextFactory.Create())
            {
                // Act
                var result = context.Query
                    ("SELECT BASISID, BASISDESCRIPTION, LEN(BASISDESCRIPTION) AS DescriptionLength " +
                    $"FROM {_tableName} WHERE BasisId = @basisId", new { basisId });

                // Assert
                Assert.Equal(typeof(List<dynamic>), result.GetType());
            }
        }

        [Fact]
        public void Query_ReturnsExpectedItem_WhenParamsExist()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            const long basisId = 0;
            var basis = new Basis(basisId, "BasisCode0", "BasisDescription0");

            using (IContext context = _contextFactory.Create())
            {
                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ("SELECT BASISID, BASISCODE, BASISDESCRIPTION " +
                       $"FROM {_tableName} " +
                        "WHERE BasisId = @basisId", new { basisId });
                var retrievedBasis = basisList[0];

                // Assert
                Assert.Single(basisList);
                Assert.Equal(basis.BasisId, retrievedBasis.BasisId);
                Assert.Equal(basis.BasisCode, retrievedBasis.BasisCode);
                Assert.Equal(basis.BasisDescription, retrievedBasis.BasisDescription);
            }
        }

        [Fact]
        public void Query_ReturnsExpectedItem_WhenNoParams()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(3);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ($"SELECT BASISID, BASISCODE, BASISDESCRIPTION FROM {_tableName}");

                // Assert
                Assert.Equal(3, basisList.Count);

                Assert.Contains(basisList, b => b.BasisId == 0 && b.BasisCode == "BasisCode0" && b.BasisDescription == "BasisDescription0");
                Assert.Contains(basisList, b => b.BasisId == 1 && b.BasisCode == "BasisCode1" && b.BasisDescription == "BasisDescription1");
                Assert.Contains(basisList, b => b.BasisId == 2 && b.BasisCode == "BasisCode2" && b.BasisDescription == "BasisDescription2");
            }
        }

        [Fact]
        public void Execute_DoesNotInsertValue_IfTransactionNotCommitted()
        {
            // Arrange
            TestDataCleanup();

            const long basisId = -1;

            // Act
            using (IContext context = _contextFactory.Create())
            {
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");
                context.Execute($"INSERT INTO {_tableName}(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                 "VALUES(@basisId, @basisCode, @basisDescription)", basis);
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var result = context.Query<Basis>
                    ($"SELECT * FROM {_tableName} WHERE " +
                      "BasisId = @basisId", new { basisId });
                Assert.Empty(result);
            }
        }

        [Fact]
        public void Execute_DeletesEntity()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                context.Execute($"DELETE FROM {_tableName} WHERE BASISID = @basisId", new { basisId = -1 });

                var result = context.Query<Basis>
                    ($"SELECT * FROM {_tableName} WHERE BasisId = @basisId", new { basisId = -1 });

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public void Execute_UpdatesEntity()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(1);

            var expectedUpdatedBasis = new Basis(0, "NewBasisCode", "NewBasisDescription");

            using (IContext context = _contextFactory.Create())
            {
                // Act
                context.Execute($"UPDATE {_tableName} SET BASISCODE = @newBasisCode, " +
                                 "BASISDESCRIPTION = @newBasisDescription WHERE BASISID = @newBasisId", 
                                new { newBasisId = expectedUpdatedBasis.BasisId, 
                                      newBasisCode = expectedUpdatedBasis.BasisCode, 
                                      newBasisDescription = expectedUpdatedBasis.BasisDescription });

                var result = context.Query<Basis>
                    ($"SELECT * FROM {_tableName} WHERE BasisId = @basisId", new { basisId = 0 }).First();

                // Assert
                result.Should().BeEquivalentTo(expectedUpdatedBasis);
            }
        }

        [Fact]
        public void ExecuteScalar_ReturnsCorrectRowCount()
        {
            // Arrange
            TestDataCleanup();
            MultipleObjectInsertion(3);

            using (IContext context = _contextFactory.Create())
            {
                // Act
                var result = context.ExecuteScalar<int>($"SELECT COUNT(*) FROM {_tableName}");

                // Assert
                Assert.Equal(3, result);
            }
        }

        [Fact]
        public void Commit_SuccesfullyCommits()
        {
            // Arrange
            TestDataCleanup();

            const long basisId = -1;
            using (IContext context = _contextFactory.Create())
            {
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");
                context.Execute($"INSERT INTO {_tableName}(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                 "VALUES(@basisId, @basisCode, @basisDescription)", basis);

                // Act
                context.Commit();
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var result = context.Query<Basis>
                    ($"SELECT * FROM {_tableName} WHERE " +
                      "BasisId = @basisId", new { basisId });
                
                Assert.NotEmpty(result);

                // Teardown
                context.Execute($"DELETE FROM {_tableName} WHERE BASISID = @basisId", new { basisId });
            }
        }

        [Fact]
        public void Dispose_SuccesfullyDisposesConnectionAndTransaction()
        {
            // Arrange
            TestDataCleanup();

            IContext context = _contextFactory.Create();
            var basis = new Basis(-1, "BasisCode", "BasisDescription");

            context.Execute($"INSERT INTO {_tableName}(BASISID, BASISCODE, BASISDESCRIPTION) " +
                             "VALUES(@basisId, @basisCode, @basisDescription)", basis);
            // Act
            context.Dispose();

            // Assert
            Assert.Throws<InvalidOperationException>(() => 
            context.Query<Basis>($"SELECT * FROM {_tableName}"));
        }

        private static string GetJsonDirectoryPath()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directoryInfo != null && !directoryInfo.GetFiles("appsettings.basistest.json").Any())
                directoryInfo = directoryInfo.Parent;

            return directoryInfo?.FullName ?? throw new FileNotFoundException("Configuration file not found.");
        }

        private void TestDataCleanup()
        {
            using (var cleanupContext = _contextFactory.Create())
            {
                cleanupContext.Execute($"IF OBJECT_ID('{_tableName}', 'U') " +
                                       $"IS NOT NULL TRUNCATE TABLE {_tableName}");
                cleanupContext.Commit();
            }
        }

        private void MultipleObjectInsertion(int numberOfObjectsToBeInserted)
        {
            using (IContext context = _contextFactory.Create())
            {
                for (int objectNumber = 0; objectNumber < numberOfObjectsToBeInserted; objectNumber++)
                {
                    context.Execute($"INSERT INTO {_tableName}(BASISID, BASISCODE, BASISDESCRIPTION) " +
                            "VALUES(@basisId, @basisCode, @basisDescription)", new Basis(objectNumber, 
                                                                                $"BasisCode{objectNumber}", 
                                                                                $"BasisDescription{objectNumber}"));
                }

                context.Commit();
            }
        }
    }
}