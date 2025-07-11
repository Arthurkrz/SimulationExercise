using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IConsistentReadingExportDTOFactory
    {
        IList<ConsistentReadingExportDTO> CreateExportDTOList(IList<ConsistentReadingGetDTO> crGetDTOs);
    }
}
