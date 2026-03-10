using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Services.Factories
{
    public class AverageProvinceDataExportDTOFactory : IAverageProvinceDataExportDTOFactory
    {
        public IList<AverageProvinceDataExportDTO> CreateExportDTOList(IList<AverageProvinceDataGetDTO> apdGetDTOs) =>
            apdGetDTOs.Select(x => new AverageProvinceDataExportDTO(
                              x.Province, x.SensorTypeName, x.AverageValue,
                              x.Unit.ToString(), x.AverageDaysOfMeasure)).ToList();
    }
}
