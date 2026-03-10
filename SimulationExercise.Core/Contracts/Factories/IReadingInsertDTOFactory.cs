using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IReadingInsertDTOFactory
    {
        List<ReadingInsertDTO> CreateReadingInsertDTOList(IList<Reading> readings, long inputFileId);
    }
}
