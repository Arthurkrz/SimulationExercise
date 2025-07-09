using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services
{
    public class ConsistentReadingService : IConsistentReadingService
    {
        private readonly IConsistentReadingFactory _consistentReadingFactory;
        private readonly IConsistentReadingInsertDTOFactory _consistentReadingInsertDTOFactory;
        private readonly IContextFactory _contextFactory;
        private readonly IReadingRepository _readingRepository;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private ILogger<ConsistentReadingService> _logger;

        public ConsistentReadingService(IConsistentReadingFactory consistentReadingFactory,
                                        IConsistentReadingInsertDTOFactory consistentReadingInsertDTOFactory,
                                        IContextFactory contextFactory,
                                        IReadingRepository readingRepository, 
                                        IConsistentReadingRepository consistentReadingRepository, 
                                        ILogger<ConsistentReadingService> logger)
        {
            _consistentReadingFactory = consistentReadingFactory;
            _consistentReadingInsertDTOFactory = consistentReadingInsertDTOFactory;
            _contextFactory = contextFactory;
            _readingRepository = readingRepository;
            _consistentReadingRepository = consistentReadingRepository;
            _logger = logger;
        }

        public void ProcessReadings()
        {
            using (IContext context = _contextFactory.Create())
            {
                var readingDTOs = _readingRepository.GetByStatus(Status.New, context);

                if (readingDTOs.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Reading");
                    return;
                }

                List<ConsistentReadingInsertDTO> insertDTOs = null;

                foreach (var readingDTO in readingDTOs)
                {
                    var reading = new Reading(readingDTO.SensorId, readingDTO.SensorTypeName,
                                              readingDTO.Unit, readingDTO.StationId,
                                              readingDTO.StationName, readingDTO.Value,
                                              readingDTO.Province, readingDTO.City,
                                              readingDTO.IsHistoric, readingDTO.StartDate,
                                              readingDTO.StopDate, readingDTO.UtmNord,
                                              readingDTO.UtmEst, readingDTO.Latitude,
                                              readingDTO.Longitude);

                    var creationResult = _consistentReadingFactory
                        .CreateConsistentReading(reading);

                    if (creationResult.Success)
                    {
                        var insertDTO = _consistentReadingInsertDTOFactory.
                            CreateConsistentReadingInsertDTO(creationResult.Value, 
                                                             readingDTO.ReadingId);
                        insertDTOs.Add(insertDTO);
                        continue;
                    }
                    
                    var updateDTO = new ReadingUpdateDTO(readingDTO.ReadingId,
                                                         Status.Error,
                                                         creationResult.Errors);

                    _readingRepository.Update(updateDTO, context);
                    continue;
                }

                if (insertDTOs.Count > 0) foreach (var insertDTO in insertDTOs)
                    _consistentReadingRepository.Insert(insertDTO, context);
            }
        }
    }
}
