using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IConsistentReadingFactory
    {
        Result<ConsistentReading> CreateConsistentReading(Reading reading);
    }
}