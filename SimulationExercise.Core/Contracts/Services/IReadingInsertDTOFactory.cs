using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IReadingInsertDTOFactory
    {
        List<ReadingInsertDTO> CreateReadingInsertDTOList(IList<Reading> readings, long inputFileId);
    }
}
