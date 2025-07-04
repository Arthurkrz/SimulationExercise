using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface ITestRepositoryObjectInsertion<T>
    {
        void InsertObjects(int numberOfObjectsToBeInserted, Status status = Status.New);
    }
}
