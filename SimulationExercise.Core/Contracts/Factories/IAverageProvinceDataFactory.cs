using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IAverageProvinceDataFactory
    {
        IList<Result<AverageProvinceData>> CreateAverageProvinceData(IList<ConsistentReading> consistentReadings);
    }
}
