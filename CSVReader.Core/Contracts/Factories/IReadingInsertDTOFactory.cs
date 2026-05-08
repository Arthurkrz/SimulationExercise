using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;

namespace CSVReader.Core.Contracts.Factories
{
    public interface IReadingInsertDTOFactory
    {
        List<ReadingInsertDTO> CreateReadingInsertDTOList(IList<Reading> readings, long inputFileId);
    }
}
