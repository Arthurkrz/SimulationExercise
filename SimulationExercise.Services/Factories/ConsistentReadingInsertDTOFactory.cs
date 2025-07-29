using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services.Factories
{
    public class ConsistentReadingInsertDTOFactory : IConsistentReadingInsertDTOFactory
    {
        public ConsistentReadingInsertDTO CreateConsistentReadingInsertDTO(ConsistentReading cr, long readingId)
        {
            return new ConsistentReadingInsertDTO(readingId, cr.SensorId, cr.SensorTypeName,
                                                  cr.Unit, cr.Value, cr.Province, cr.City,
                                                  cr.IsHistoric, cr.DaysOfMeasure, 
                                                  cr.UtmNord, cr.UtmEst, cr.Latitude, 
                                                  cr.Longitude, Status.New);
        }
    }
}
