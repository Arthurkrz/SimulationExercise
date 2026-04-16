using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Services.Factories
{
    public class ConsistentReadingExportDTOFactory : IConsistentReadingExportDTOFactory
    {
        public IList<ConsistentReadingExportDTO> CreateExportDTOList(IList<ConsistentReadingGetDTO> crGetDTOs) =>
            crGetDTOs.Select(x => new ConsistentReadingExportDTO(
                             x.SensorId, x.SensorTypeName, x.Unit.ToString(),
                             x.Value, x.Province, x.City, x.IsHistoric,
                             x.DaysOfMeasure, x.UtmNord, x.UtmEst,
                             x.Latitude, x.Longitude)).ToList();
    }
}
