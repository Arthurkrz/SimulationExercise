using SimulationExercise.Core.CSVDTOs;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IConsistentReadingExportDTOFactory
    {
        IList<ConsistentReadingExportDTO> CreateExportDTOList(IList<ConsistentReadingGetDTO> crGetDTOs);
    }
}
