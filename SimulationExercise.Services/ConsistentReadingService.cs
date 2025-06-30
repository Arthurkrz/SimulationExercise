using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using System.Text;
using System.Text.Json;

namespace SimulationExercise.Services
{
    public class ConsistentReadingService : IConsistentReadingService
    {
        private readonly IConsistentReadingFactory _consistentReadingFactory;
        private readonly IContextFactory _contextFactory;
        private readonly IReadingRepository _readingRepository;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private readonly IOutputFileService _outputFileService;

        public ConsistentReadingService(IConsistentReadingFactory consistentReadingFactory,
                                        IContextFactory contextFactory,
                                        IReadingRepository readingRepository, 
                                        IConsistentReadingRepository consistentReadingRepository, 
                                        IOutputFileService outputFileService)
        {
            _consistentReadingFactory = consistentReadingFactory;
            _contextFactory = contextFactory;
            _readingRepository = readingRepository;
            _consistentReadingRepository = consistentReadingRepository;
            _outputFileService = outputFileService;
        }

        public void ProcessReadings(IList<ReadingGetDTO> readingDTOs)
        {
            foreach (var dto in readingDTOs)
            {
                string json = Encoding.UTF8.GetString(dto.Bytes);
                Reading reading = JsonSerializer.Deserialize<Reading>(json);

                var creationResult = _consistentReadingFactory
                    .CreateConsistentReading(reading);

                if (creationResult.Success)
                {
                    string consistentReadingJson = JsonSerializer
                        .Serialize(creationResult.Value);

                    byte[] consistentReadingBytes = Encoding.UTF8
                        .GetBytes(consistentReadingJson);

                    var consistentReadingInsert = new ConsistentReadingInsertDTO
                        (dto.ReadingId, consistentReadingBytes, Status.New);

                    using (IContext context = _contextFactory.Create())
                    {
                        _consistentReadingRepository.Insert
                            (consistentReadingInsert, context);
                    }
                }
                else
                {
                    using (IContext context = _contextFactory.Create())
                    {
                        var readingUpdate = new ReadingUpdateDTO
                                                (dto.InputFileId, 
                                                Status.Error, 
                                                creationResult.Errors);

                        _readingRepository.Update(readingUpdate, context);
                    }

                }
            }

            using (IContext context = _contextFactory.Create())
            {
                var consistentReadings = _consistentReadingRepository
                                .GetByStatus(Status.New, context);

                if (consistentReadings.Any())
                    _outputFileService.ProcessConsistentReadings
                                        (consistentReadings);
            }
        }
    }
}
