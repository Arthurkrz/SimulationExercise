using SimulationExercise.Core.Contracts.Repository;

namespace SimulationExercise.Architecture
{
    public class DapperRepositoryInitializer : IRepositoryInitializer
    {
        private readonly string _tableName = "BasisData";

        public void Initialize(IContext context) => TestBasisDatabaseTableCreationIfNotCreated(context);

        private void TestBasisDatabaseTableCreationIfNotCreated(IContext context)
        {
            string mainTableCreationQuery =
                $"IF OBJECT_ID('{_tableName}', 'U') IS NULL " +
                $"CREATE TABLE {_tableName} " +
                    "(BASISID BIGINT PRIMARY KEY, " +
                    "BASISCODE NVARCHAR(50) NOT NULL, " +
                    "BASISDESCRIPTION NVARCHAR(255) NOT NULL);";

            context.Execute(mainTableCreationQuery);
        }
    }
}
