using FileHelpers;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Services
{
    public class ReadingImportService : IReadingImportService
    {
        public ImportResult Import(Stream stream)
        {
            if (stream == null) throw new FormatException("Empty stream.");

            var engine = new FileHelperEngine<ReadingDTO>();
            engine.ErrorMode = ErrorMode.SaveAndContinue;

            using (var sr = new StreamReader(stream))
            {
                var currentHeader = sr.ReadLine().Replace(" ", "");
                var header = engine.GetFileHeader();

                if (!header.Equals(currentHeader, StringComparison.OrdinalIgnoreCase))
                    throw new FormatException("Invalid header values.");

                var engineRecords = engine.ReadStream(sr);
                var engineErrors = engine.ErrorManager.Errors;
                List<string> errorString = new List<string>();

                foreach (var errorInfo in engineErrors) 
                    errorString.Add(errorInfo.ExceptionInfo.Message);

                var recordReading = engineRecords.Select(r => 
                                             new Reading(r.IdSensore, r.NomeTipoSensore,
                                                         r.UnitaMisura, r.Idstazione,
                                                         r.NomeStazione, r.Quota,
                                                         r.Provincia, r.Comune,
                                                         IsHistoric(r.Storico),
                                                         r.DataStart, r.DataStop,
                                                         r.Utm_Nord, r.UTM_Est,
                                                         r.lat, r.lng)).ToList();

                return new ImportResult(recordReading, errorString);
            }
        }

        private bool IsHistoric(string storico)
        {
            return storico.Equals("S", StringComparison.OrdinalIgnoreCase);
        }
    }
}