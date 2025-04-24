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
                    throw new FormatException("Invalid header values.");
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
                    bool readingIsHistoric = IsHistoric(record.Storico);
                    Reading reading = new Reading(
                        record.IdSensore,
                        record.NomeTipoSensore,
                        record.UnitaMisura,
                        record.Idstazione,
                        record.NomeStazione,
                        record.Quota,
                        record.Provincia,
                        record.Comune,
                        readingIsHistoric,
                        record.DataStart,
                        record.DataStop,
                        record.Utm_Nord,
                        record.UTM_Est,
                        record.lat,
                        record.lng
                    );

                    recordReading.Add(reading);
                }

                return new ImportResult(recordReading, errorString);
            }
        }

        private bool IsHistoric(string storico)
        {
            return storico.Equals("S", StringComparison.OrdinalIgnoreCase);
        }
    }
}