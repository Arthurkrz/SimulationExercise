using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.CSVDTOs;
using SimulationExercise.Core.DTOS;

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
