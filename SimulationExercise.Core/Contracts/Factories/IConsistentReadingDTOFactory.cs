using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IConsistentReadingInsertDTOFactory
    {
        ConsistentReadingInsertDTO CreateConsistentReadingInsertDTO(ConsistentReading cr, long readingId);
    }
}
