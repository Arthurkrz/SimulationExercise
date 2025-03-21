using FileHelpers;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimulationExercise.Services
{
    public class ReadingImportService : IReadingImportService
    {
        public ImportResult Import(Stream stream)
        {
            stream.Position = 0;

            var engine = new FileHelperEngine<ReadingDTO>();
            engine.ErrorMode = ErrorMode.SaveAndContinue;

            var header = engine.GetFileHeader();
            if (!IsValidHeader(header))
            {
                throw new FormatException("Invalid header format.");
            }

            var engineErrors = engine.ErrorManager.Errors;
            List<string> errorString = new List<string>();

            var streamReader = new StreamReader(stream, leaveOpen: true);
            var engineRecords = engine.ReadStream(streamReader);

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


        private bool IsValidHeader(string header)
		{
			List<string> headerElements = header.Split(',')
												.ToList();
      
            List<string> readingProperties = typeof(Reading).GetProperties()
                                                            .Select(prop => prop.Name)
                                                            .ToList();

            return headerElements.SequenceEqual(readingProperties);
		}
	}
}
