using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Core.Contracts.Factories
{
    public interface IAverageProvinceDataExportDTOFactory 
    {
        IList<AverageProvinceDataExportDTO> CreateExportDTOList(IList<AverageProvinceDataGetDTO> apdGetDTOs);
    }
}
