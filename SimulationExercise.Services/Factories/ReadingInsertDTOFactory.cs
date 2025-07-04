using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services.Factories
{
    public class ReadingInsertDTOFactory : IReadingInsertDTOFactory
    {
        public List<ReadingInsertDTO> CreateReadingInsertDTOList(IList<Reading> readings, long inputFileId)
        {
            List<ReadingInsertDTO> readingInsertDTOs = new List<ReadingInsertDTO>();
            foreach (var reading in readings)
            {
                Unit unit = default;

                if (reading.Unit == "ng/m³") unit = Unit.ng_m3;
                if (reading.Unit == "mg/m³") unit = Unit.mg_m3;
                if (reading.Unit == "µg/m³") unit = Unit.µg_m3;

                readingInsertDTOs.Add(new ReadingInsertDTO(
                                        inputFileId, reading.SensorId, reading.SensorTypeName,
                                        (int)unit, reading.StationId, reading.StationName, reading.Value,
                                        reading.Province, reading.City, reading.IsHistoric, 
                                        reading.StartDate, reading.StopDate, reading.UtmNord, 
                                        reading.UtmEst, reading.Latitude, reading.Longitude, Status.Success));
            }

            return readingInsertDTOs;
        }
    }
}
