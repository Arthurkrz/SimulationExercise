using SimulationExercise.Core.CSVDTOs;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IAverageProvinceDataExportDTOFactory 
    {
        IList<AverageProvinceDataExportDTO> CreateExportDTOList(IList<AverageProvinceDataGetDTO> apdGetDTOs);
    }
}
