using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts
{
    public interface IAverageProvinceDataFactory
    {
        Result<AverageProvinceData> CreateAverageProvinceData(ProvinceData provinceData);
    }
}
