using CSVReader.Core.Entities;

namespace CSVReader.Core.Contracts.Factories
{
    public interface IConsistentReadingFactory
    {
        Result<ConsistentReading> CreateConsistentReading(Reading reading);
    }
}