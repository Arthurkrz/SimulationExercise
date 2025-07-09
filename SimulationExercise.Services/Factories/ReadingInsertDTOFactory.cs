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
                readingInsertDTOs.Add(new ReadingInsertDTO(
                                        inputFileId, reading.SensorId, reading.SensorTypeName,
                                        reading.Unit, reading.StationId, reading.StationName, 
                                        reading.Value, reading.Province, reading.City, reading.IsHistoric, 
                                        reading.StartDate, reading.StopDate, reading.UtmNord, 
                                        reading.UtmEst, reading.Latitude, reading.Longitude, Status.Success));

            return readingInsertDTOs;
        }
    }
}
