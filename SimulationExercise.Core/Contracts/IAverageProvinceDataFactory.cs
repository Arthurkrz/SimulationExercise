namespace SimulationExercise.Core.Contracts
{
    public interface IAverageProvinceDataFactory
    {
        Result<AverageProvinceData> CreateAverageProvinceData
                                   (ProvinceData provinceData);
    }
}
