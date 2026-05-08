using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;

namespace CSVReader.Core.Contracts.Factories
{
    public interface IConsistentReadingInsertDTOFactory
    {
        ConsistentReadingInsertDTO CreateConsistentReadingInsertDTO(ConsistentReading cr, long readingId);
    }
}
