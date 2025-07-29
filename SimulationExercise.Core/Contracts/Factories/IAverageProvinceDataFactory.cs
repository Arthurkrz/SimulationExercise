using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IAverageProvinceDataFactory
    {
        Result<AverageProvinceData> CreateAverageProvinceData(ProvinceData provinceData);
    }
}
