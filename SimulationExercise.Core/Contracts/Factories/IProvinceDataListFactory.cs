using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IProvinceDataListFactory
    {
        IList<ProvinceData> CreateProvinceDataList(IList<ConsistentReading> consistentReadings);
    }
}