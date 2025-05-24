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

        }

        [Fact]
        public void Query_ReturnsExpectedItem_WhenParamsExist()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["XXX"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);

            using (IContext context = contextFactory.Create())
            {
                const string basisCode = "BasisCode";
                const string basisDescription = "BasisDescription";
                const long basisId = -1;
                context.Execute(
                    "INSERT INTO DBO.BASIS(BASISID, BASISCODE, BASISDESCRIPTION)" +
                    "VALUES(@basisId, @basisCode, @basisDescription)",
                    new Basis(basisId, basisCode, basisDescription));

                // Act
                IList<Basis> basisList = context.Query<Basis>
                    ("SELECT BASISID, BASISCODE, BASISDESCRIPTION" +
                        "FROM dbo.Basis" +
                        "WHERE = BasisId = @basisId", new { basisId });
                var retrievedBasis = basisList[0];

                // Assert
                Assert.Equal(1, basisList.Count);
                Assert.Equal(basisId, retrievedBasis.BasisId);
                Assert.Equal(basisCode, retrievedBasis.BasisCode);
                Assert.Equal(basisDescription, retrievedBasis.BasisDescription);
            }
        }

        [Fact]
        public void Query_ReturnsExpectedItem_WhenNoParams()
        {

        }

        [Fact]
        public void Execute_DoesNotEditTable_IfTransactionNotCommitted()
        {

        }

        [Fact]
        public void Execute_InsertsEntity_WhenCommitted()
        {

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

        }

        [Fact]
        public void Dispose_RollsBackTransactionIfNotCommitted()
        {

        }

        [Fact]
        public void Dispose_SuccesfullyDisposesConnectionAndTransaction()
        {

        }
    }
}