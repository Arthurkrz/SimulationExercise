
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryIntegrationTest
    {
        private readonly string _connectionString;
        private IContextFactory _contextFactory;

        public RepositoryIntegrationTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            _connectionString = config.GetConnectionString("Test");
            _contextFactory = new DapperContextFactory(_connectionString);
        }

        [Fact]
        public void Query_ReturnsExpectedDynamicItem()
        {
            // Arrange
            TestDataCleanup();
            const long basisId = -1;
            var basis = new Basis(-1, "BasisCode", "BasisDescription");

            using (IContext context = _contextFactory.Create())
            {
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);
                // Act
                var result = context.Query
                    ("SELECT BASISID, BASISDESCRIPTION, LEN(BASISDESCRIPTION) AS DescriptionLength " +
                     "FROM dbo.BasisDataTest WHERE BasisId = @basisId", new { basisId });
                var retrievedBasis = result[0];

                // Assert
                Assert.Equal(basis.BasisId, (long)retrievedBasis.BASISID);
                Assert.Equal(basis.BasisDescription, (string)retrievedBasis.BASISDESCRIPTION);
                Assert.Equal(basis.BasisDescription.Length, (int)retrievedBasis.DescriptionLength);
            }
        }

        [Fact]
        public void Query_ReturnsExpectedItem_WhenParamsExist()
        {
            // Arrange
            TestDataCleanup();

            using (IContext context = _contextFactory.Create())
            {
                const long basisId = -1;
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");

                context.Execute
                    ("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                     "VALUES(@basisId, @basisCode, @basisDescription)", basis);

                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ("SELECT BASISID, BASISCODE, BASISDESCRIPTION " +
                        "FROM dbo.BasisDataTest " +
                        "WHERE BasisId = @basisId", new { basisId });
                var retrievedBasis = basisList[0];

                // Assert
                Assert.Equal(1, basisList.Count);
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

            var b1 = new Basis(-1, "BasisCode", "BasisDescription");
            var b2 = new Basis(-2, "BasisCode2", "BasisDescription2");
            var b3 = new Basis(-3, "BasisCode3", "BasisDescription3");

            using (IContext context = _contextFactory.Create())
            {
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b1);
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b2);
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b3);

                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ("SELECT BASISID, BASISCODE, BASISDESCRIPTION FROM dbo.BasisDataTest");

                // Assert
                Assert.Equal(3, basisList.Count);

                Assert.Contains(basisList, b => b.BasisId == b1.BasisId && b.BasisCode == b1.BasisCode && b.BasisDescription == b1.BasisDescription);
                Assert.Contains(basisList, b => b.BasisId == b2.BasisId && b.BasisCode == b2.BasisCode && b.BasisDescription == b2.BasisDescription);
                Assert.Contains(basisList, b => b.BasisId == b3.BasisId && b.BasisCode == b3.BasisCode && b.BasisDescription == b3.BasisDescription);
            }
        }

        [Fact]
        public void Execute_DoesNotEditTable_IfTransactionNotCommitted()
        {
            // Arrange
            TestDataCleanup();

            const long basisId = -1;

            // Act
            using (IContext context = _contextFactory.Create())
            {
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var result = context.Query<Basis>
                    ("SELECT * FROM dbo.BasisDataTest WHERE " +
                     "BasisId = @basisId", new { basisId });
                Assert.Empty(result);
            }
        }

        [Fact]
        public void Execute_DeletesEntity()
        {
            // Arrange
            TestDataCleanup();

            const long basisId = -1;
            var basis = new Basis(basisId, "BasisCode", "BasisDescription");

            using (IContext context = _contextFactory.Create())
            {
                // Act
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);

                context.Execute("DELETE FROM dbo.BasisDataTest WHERE BASISID = @basisId", new { basisId });

                var result = context.Query<Basis>
                    ("SELECT * FROM dbo.BasisDataTest WHERE BasisId = @basisId", new { basisId });

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public void Execute_UpdatesEntity()
        {
            // Arrange
            TestDataCleanup();

            const long basisId = -1;
            var basis = new Basis(basisId, "BasisCode", "BasisDescription");
            var expectedUpdatedBasis = new Basis(basisId, "NewBasisCode", "NewBasisDescription");

            using (IContext context = _contextFactory.Create())
            {
                // Act
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);

                context.Execute("UPDATE dbo.BasisDataTest SET BASISCODE = @newBasisCode, " +
                                "BASISDESCRIPTION = @newBasisDescription WHERE BASISID = @basisId", 
                                new { basisId, newBasisCode = expectedUpdatedBasis.BasisCode, 
                                               newBasisDescription = expectedUpdatedBasis.BasisDescription });

                var result = context.Query<Basis>
                    ("SELECT * FROM dbo.BasisDataTest WHERE BasisId = @basisId", new { basisId }).First();

                // Assert
                result.Should().BeEquivalentTo(expectedUpdatedBasis);
                Assert.Equal(expectedUpdatedBasis.BasisId, result.BasisId);
                Assert.Equal(expectedUpdatedBasis.BasisCode, result.BasisCode);
                Assert.Equal(expectedUpdatedBasis.BasisDescription, result.BasisDescription);
            }
        }

        [Fact]
        public void ExecuteScalar_ReturnsCorrectRowCount()
        {
            // Arrange
            TestDataCleanup();

            var b1 = new Basis(-1, "BasisCode", "BasisDescription");
            var b2 = new Basis(-2, "BasisCode2", "BasisDescription2");
            var b3 = new Basis(-3, "BasisCode3", "BasisDescription3");

            using (IContext context = _contextFactory.Create())
            {
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b1);
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b2);
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b3);

                // Act
                var result = context.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.BasisDataTest");

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

            // Act
            using (IContext context = _contextFactory.Create())
            {
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");
                context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);
                context.Commit();
            }

            // Assert
            using (IContext context = _contextFactory.Create())
            {
                var result = context.Query<Basis>
                    ("SELECT * FROM dbo.BasisDataTest WHERE " +
                     "BasisId = @basisId", new { basisId });
                
                Assert.NotEmpty(result);

                // Teardown
                context.Execute("DELETE FROM dbo.BasisDataTest WHERE BASISID = @basisId", new { basisId });
            }
        }

        [Fact]
        public void Dispose_SuccesfullyDisposesConnectionAndTransaction()
        {
            // Arrange
            TestDataCleanup();

            IContext context = _contextFactory.Create();
            var basis = new Basis(-1, "BasisCode", "BasisDescription");

            // Act
            context.Execute("INSERT INTO dbo.BasisDataTest(BASISID, BASISCODE, BASISDESCRIPTION) " +
                            "VALUES(@basisId, @basisCode, @basisDescription)", basis);
            context.Dispose();

            // Assert
            Assert.Throws<InvalidOperationException>(() => 
            context.Query<Basis>("SELECT * FROM dbo.BasisDataTest"));
        }

        private void TestDataCleanup()
        {
            using (var cleanupContext = _contextFactory.Create())
            {
                cleanupContext.Execute("DELETE FROM dbo.BasisDataTest");
                cleanupContext.Commit();
            }
        }
    }
}