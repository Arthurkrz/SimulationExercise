using System.Configuration;
using SimulationExercise.Architecture;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryIntegrationTest
    {
        [Fact]
        public void Query_ReturnsExpectedDynamicItem()
        {
            // Arrange
            string connectionString = ConfigurationManager.ConnectionStrings["Server=localhost;Database=BasisDb_Test;Trusted_Connection=True;"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);

            using (IContext context = contextFactory.Create())
            {
                var basis = new Basis(-1, "BasisCode", "BasisDescription");

                context.Execute("INSERT INTO dbo.Basis(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);
                // Act
                var result = context.Query
                    ("SELECT BASISID, BASISDESCRIPTION, LEN(BASISDESCRIPTION) AS DescriptionLength " +
                     "FROM dbo.Basis WHERE BasisId = @basisId", new { id = basis.BasisId }).First();

                // Assert
                Assert.Equal(basis.BasisId, result.BasisId);
                Assert.Equal(basis.BasisDescription, result.BasisDescription);
                Assert.Equal(basis.BasisDescription.Length, result.DescriptionLength);
            }
        }

        [Fact]
        public void Query_ReturnsExpectedItem_WhenParamsExist()
        {
            // Arrange
            string connectionString = ConfigurationManager.ConnectionStrings["Server=localhost;Database=BasisDb_Test;Trusted_Connection=True;"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);

            using (IContext context = contextFactory.Create())
            {
                const long basisId = -1;
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");

                context.Execute
                    ("INSERT INTO DBO.BASIS(BASISID, BASISCODE, BASISDESCRIPTION) " +
                     "VALUES(@basisId, @basisCode, @basisDescription)", basis);

                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ("SELECT BASISID, BASISCODE, BASISDESCRIPTION" +
                        "FROM dbo.Basis" +
                        "WHERE = BasisId = @basisId", new { basisId });
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
            string connectionString = ConfigurationManager.ConnectionStrings["Server=localhost;Database=BasisDb_Test;Trusted_Connection=True;"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);

            using (IContext context = contextFactory.Create())
            {
                var b1 = new Basis(-1, "BasisCode", "BasisDescription");
                var b2 = new Basis(-2, "BasisCode2", "BasisDescription2");
                var b3 = new Basis(-3, "BasisCode3", "BasisDescription3");
                context.Execute("INSERT INTO dbo.Basis(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b1);
                context.Execute("INSERT INTO dbo.Basis(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b2);
                context.Execute("INSERT INTO dbo.Basis(BASISID, BASISCODE, BASISDESCRIPTION) VALUES(@basisId, @basisCode, @basisDescription)", b3);

                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ("SELECT BASISID, BASISCODE, BASISDESCRIPTION FROM dbo.Basis");

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
            string connectionString = ConfigurationManager.ConnectionStrings["Server=localhost;Database=BasisDb_Test;Trusted_Connection=True;"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);
            const long basisId = -1;

            // Act
            using (IContext context = contextFactory.Create())
            {
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");
                context.Execute("INSERT INTO dbo.Basis(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);
            }

            // Assert
            using (IContext context = contextFactory.Create())
            {
                var result = context.Query<Basis>
                    ("SELECT * FROM dbo.Basis WHERE " +
                     "BasisId = @basisId", new { basisId });
                Assert.Empty(result);
            }
        }

        [Fact]
        public void Execute_DeletesEntity_WhenCommitted()
        {

        }

        [Fact]
        public void Execute_UpdatesEntity_WhenComitted()
        {

        }

        [Fact]
        public void ExecuteScalar_ReturnsCorrectRowCount()
        {

        }

        [Fact]
        public void Commit_SuccesfullyCommits()
        {
            // Arrange
            string connectionString = ConfigurationManager.ConnectionStrings["Server=localhost;Database=BasisDb_Test;Trusted_Connection=True;"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);
            const long basisId = -1;

            // Act
            using (IContext context = contextFactory.Create())
            {
                var basis = new Basis(basisId, "BasisCode", "BasisDescription");
                context.Execute("INSERT INTO dbo.Basis(BASISID, BASISCODE, BASISDESCRIPTION) " +
                                "VALUES(@basisId, @basisCode, @basisDescription)", basis);
                context.Commit();
            }

            // Assert
            using (IContext context = contextFactory.Create())
            {
                var result = context.Query<Basis>
                    ("SELECT * FROM dbo.Basis WHERE " +
                     "BasisId = @basisId", new { basisId });
                
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void Dispose_SuccesfullyDisposesConnectionAndTransaction()
        {
            // Arrange
            string connectionString = ConfigurationManager.ConnectionStrings["Server=localhost;Database=BasisDb_Test;Trusted_Connection=True;"].ConnectionString;
            IContext context = new DapperContextFactory(connectionString).Create();
            context.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => 
                context.Query<Basis>("SELECT * FROM dbo.Basis"));
        }
    }
}