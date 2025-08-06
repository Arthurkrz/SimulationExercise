using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.CSVDTOs;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Services.Factories
{
    public class ConsistentReadingExportDTOFactory : IConsistentReadingExportDTOFactory
    {
        public IList<ConsistentReadingExportDTO> CreateExportDTOList(IList<ConsistentReadingGetDTO> crGetDTOs) =>
            crGetDTOs.Select(x => new ConsistentReadingExportDTO(
                             x.SensorId, x.SensorTypeName, x.Unit,
                             x.Value, x.Province, x.City, x.IsHistoric,
                             x.DaysOfMeasure, x.UtmNord, x.UtmEst,
                             x.Latitude, x.Longitude)).ToList();
    }
}
