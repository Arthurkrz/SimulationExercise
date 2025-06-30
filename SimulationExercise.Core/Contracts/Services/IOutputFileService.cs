using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IOutputFileService
    {
        void ProcessConsistentReadings(IList<ConsistentReadingGetDTO> consistentReading);
    }
}
