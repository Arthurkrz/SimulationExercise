using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Factories
{
    public interface IAverageProvinceDataExportDTOFactory 
    {
        IList<AverageProvinceDataExportDTO> CreateExportDTOList(IList<AverageProvinceDataGetDTO> apdGetDTOs);
    }
}
