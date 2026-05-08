using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Services.Factories
{
    public class AverageProvinceDataExportDTOFactory : IAverageProvinceDataExportDTOFactory
    {
        public IList<AverageProvinceDataExportDTO> CreateExportDTOList(IList<AverageProvinceDataGetDTO> apdGetDTOs) =>
            apdGetDTOs.Select(x => new AverageProvinceDataExportDTO(
                              x.Province, x.SensorTypeName, x.AverageValue,
                              x.Unit.ToString(), x.AverageDaysOfMeasure)).ToList();
    }
}
