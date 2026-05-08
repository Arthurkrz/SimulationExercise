using CSVReader.Core.Entities;

namespace CSVReader.Core.Contracts.Factories
{
    public interface IAverageProvinceDataFactory
    {
        IList<Result<AverageProvinceData>> CreateAverageProvinceData(IList<ConsistentReading> consistentReadings);
    }
}
