using FileHelpers;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Services
{
    public class ReadingImportService : IReadingImportService
    {
        public ImportResult Import(Stream stream)
        {

            var engine = new FileHelperEngine<ReadingDTO>();
            engine.ErrorMode = ErrorMode.SaveAndContinue;

            using (var sr = new StreamReader(stream))
            {
                var currentHeader = sr.ReadLine();
                currentHeader = currentHeader.Replace(" ", "");
                var header = engine.GetFileHeader();
                if (!header.Equals(currentHeader, StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException("Invalid header format.");
                }

                var engineRecords = engine.ReadStream(sr);
                var engineErrors = engine.ErrorManager.Errors;
                List<string> errorString = new List<string>();

                foreach (var errorInfo in engineErrors)
                {
                    errorString.Add(errorInfo.ExceptionInfo.Message);
                }

                List<Reading> recordReading = new List<Reading>();
                foreach (var record in engineRecords)
                {
                    Reading reading = new Reading(
                        record.SensorID,
                        record.SensorTypeName,
                        record.Unit,
                        record.StationId,
                        record.StationName,
                        record.Value,
                        record.Province,
                        record.City,
                        record.IsHistoric,
                        record.StartDate,
                        record.StopDate,
                        record.UtmNord,
                        record.UtmEst,
                        record.Latitude,
                        record.Longitude
                    );

                    recordReading.Add(reading);
                }

                return new ImportResult(recordReading, errorString);
            }
        }
    }
}
