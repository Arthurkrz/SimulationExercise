using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Services.Factories
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
