using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IConsistentReadingInsertDTOFactory
    {
        ConsistentReadingInsertDTO CreateConsistentReadingInsertDTOs(ConsistentReading cr, long readingId);
    }
}
