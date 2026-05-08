using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Core.Contracts.Factories
{
    public interface IConsistentReadingExportDTOFactory
    {
        IList<ConsistentReadingExportDTO> CreateExportDTOList(IList<ConsistentReadingGetDTO> crGetDTOs);
    }
}
