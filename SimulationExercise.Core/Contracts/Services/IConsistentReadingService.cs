using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IConsistentReadingService
    {
        void ProcessReadings(IList<ReadingGetDTO> readingDTOs);
    }
}
