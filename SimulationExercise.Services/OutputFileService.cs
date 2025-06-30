using FileHelpers;
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

        public void ProcessConsistentReadings(IList<ConsistentReadingGetDTO> consistentReadingDTOs)
        {
            var engine = new FileHelperEngine<ConsistentReadingExportDTO>();
            var consistentReadings = new List<ConsistentReading>();

            foreach (var consistentReadingDTO in consistentReadingDTOs)
            {
                string json = Encoding.UTF8.GetString(consistentReadingDTO.Bytes);

                ConsistentReading consistentReading = JsonSerializer
                    .Deserialize<ConsistentReading>(json);

                consistentReadings.Add(consistentReading);

                using (IContext context = _contextFactory.Create())
                {
                    var consistentReadingUpdate = new ConsistentReadingUpdateDTO
                        (consistentReadingDTO.ConsistentReadingId, Status.Success, new List<string>());
                    _consistentReadingRepository.Update(consistentReadingUpdate, context);
                }
            }

            IList<ConsistentReadingExportDTO> records = consistentReadings
                .Select(x => new ConsistentReadingExportDTO
                (x.SensorId, x.SensorTypeName, x.Unit,
                 x.Value, x.Province, x.City, x.IsHistoric,
                 x.DaysOfMeasure, x.UtmNord, x.UtmEst,
                 x.Latitude, x.Longitude)).ToList();

            var csvFile = engine.WriteString(records);
            var csvBytes = Encoding.UTF8.GetBytes(csvFile);
            var fileName = SystemTime.Now.ToString();

            var outputFileInsert = new OutputFileInsertDTO(fileName, csvBytes, ".csv", Status.Success);
        }
    }
}
