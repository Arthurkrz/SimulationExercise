using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryIntegrationTest
    {
        [Fact]
        public void Query_OneItemInserted_OneItemRetrieved()
        {
            // Arrange
            var context = SetupAndFirstInsert(out var id, out var code, out var desc);

            // Act


            // Assert

        }

        [Fact]

        private IContext SetupAndFirstInsert(out long basisId, out long basisCode, out long basisDescription)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["XXX"].ConnectionString;
            IContextFactory contextFactory = new DapperContextFactory(connectionString);
            IContext context = contextFactory.Create();

            const string basisCode = "XXX";
            const string basisDescription = "XXX";
            const long basisId = -1;
            context.Execute(
                "INSERT INTO DBO.BASIS(BASISID, BASISCODE, BASISDESCRIPTION)" +
                "VALUES(@basisId, @basisCode, @basisDescription)",
                new Basis(basisId, basisCode, basisDescription));

            return context;
        }
    }
}
