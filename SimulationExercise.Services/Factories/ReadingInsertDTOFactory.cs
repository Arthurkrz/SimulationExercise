using SimulationExercise.Core.Contracts.Factories;
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
                DateTime? stopDate = reading.StopDate;

                if (reading.StopDate == null)
                    stopDate = DateTime.Now.Date;

                readingInsertDTOs.Add(new ReadingInsertDTO(
                    inputFileId, reading.SensorId, reading.SensorTypeName,
                    reading.Unit, reading.StationId, reading.StationName,
                    reading.Value, reading.Province, reading.City, 
                    reading.IsHistoric, reading.StartDate, stopDate, 
                    reading.UtmNord, reading.UtmEst, reading.Latitude, 
                    reading.Longitude, Status.New));
            }

            return readingInsertDTOs;
        }
    }
}
