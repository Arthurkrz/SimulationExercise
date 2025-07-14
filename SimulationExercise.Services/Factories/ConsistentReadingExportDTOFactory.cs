using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Services.Factories
{
    public class ConsistentReadingExportDTOFactory : IConsistentReadingExportDTOFactory
    {
        public IList<ConsistentReadingExportDTO> CreateExportDTOList(IList<ConsistentReadingGetDTO> crGetDTOs)
        {
            return crGetDTOs.Select(x => new ConsistentReadingExportDTO(
                                    x.SensorId, x.SensorTypeName, x.Unit,
                                    x.Value, x.Province, x.City, x.IsHistoric,
                                    x.DaysOfMeasure, x.UtmNord, x.UtmEst,
                                    x.Latitude, x.Longitude)).ToList();
        }
    }
}
