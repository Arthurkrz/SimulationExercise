using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;

namespace CSVReader.Services.Factories
{
    public class ConsistentReadingInsertDTOFactory : IConsistentReadingInsertDTOFactory
    {
        public ConsistentReadingInsertDTO CreateConsistentReadingInsertDTO(ConsistentReading cr, long readingId)
        {
            return new ConsistentReadingInsertDTO(readingId, cr.SensorId, cr.SensorTypeName,
                                                  cr.Unit, cr.Value, cr.Province, cr.City,
                                                  cr.IsHistoric, cr.DaysOfMeasure, 
                                                  cr.UtmNord, cr.UtmEst, cr.Latitude, 
                                                  cr.Longitude, false);
        }
    }
}
