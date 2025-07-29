using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IConsistentReadingFactory
    {
        Result<ConsistentReading> CreateConsistentReading(Reading reading);
    }
}