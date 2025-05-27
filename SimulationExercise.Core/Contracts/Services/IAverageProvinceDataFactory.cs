using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IAverageProvinceDataFactory
    {
        Result<AverageProvinceData> CreateAverageProvinceData(ProvinceData provinceData);
    }
}
