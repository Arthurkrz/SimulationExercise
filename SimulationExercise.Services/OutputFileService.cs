using FileHelpers;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using System.Text;
using System.Text.Json;

namespace SimulationExercise.Services
{
    public class OutputFileService : IOutputFileService
    {
        private IContextFactory _contextFactory;
        private IConsistentReadingRepository _consistentReadingRepository;

        public OutputFileService(IContextFactory contextFactory, IConsistentReadingRepository consistentReadingRepository)
        {
            _contextFactory = contextFactory;
            _consistentReadingRepository = consistentReadingRepository;
        }

        public void ProcessConsistentReadings()
        {
            using (IContext context = _contextFactory.Create())
            {
                var consistentReadingDTOs = _consistentReadingRepository.GetByStatus(Status.New, context);

                if (consistentReadingDTOs.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND);
                    return;
                }

                var engine = new FileHelperEngine<ConsistentReadingExportDTO>();

                foreach (var consistentReadingDTO in consistentReadingDTOs)
                {
                    var consistentReadingUpdate = new ConsistentReadingUpdateDTO
                        (consistentReadingDTO.ConsistentReadingId, Status.Success, new List<string>());
                    _consistentReadingRepository.Update(consistentReadingUpdate, context);
                }

                IList<ConsistentReadingExportDTO> records = consistentReadingDTOs
                    .Select(x => new ConsistentReadingExportDTO
                    (x.SensorId, x.SensorTypeName, x.Unit,
                     x.Value, x.Province, x.City, x.IsHistoric,
                     x.DaysOfMeasure, x.UtmNord, x.UtmEst,
                     x.Latitude, x.Longitude)).ToList();

                string fileHeader = string.Join(",", typeof(ConsistentReading)
                    .GetFields().Select(f => f.Name));

                var csvFile = fileHeader + Environment.NewLine + engine.WriteString(records);
                var csvBytes = Encoding.UTF8.GetBytes(csvFile);
                var fileName = $"Reading{SystemTime.Now():dd_MM_yyyy}";
                var fileExtension = ".csv";

                var outputFileInsert = new OutputFileInsertDTO(fileName, csvBytes, fileExtension, Status.Success);
            }
        }
    }
}
