using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts
{
    public interface IProvinceDataListFactory
    {
        IList<ProvinceData> CreateProvinceDataList(IList<ConsistentReading> consistentReadings);
    }
}