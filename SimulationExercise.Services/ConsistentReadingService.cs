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
            IList<ConsistentReadingInsertDTO> insertDTOs = null;
            IList<ReadingGetDTO> readingDTOs = new List<ReadingGetDTO>();  
            using (IContext searchContext = _contextFactory.Create())
            {
                readingDTOs = _readingRepository.GetByStatus(Status.New, searchContext);
            }

            if (readingDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Reading");
                return;
            }

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

                using (IContext context = _contextFactory.Create())
                {
                    if (!creationResult.Success)
                    {
                        var updateDTO = new ReadingUpdateDTO(readingDTO.ReadingId,
                                                             Status.Error,
                                                             creationResult.Errors);

                        try { _readingRepository.Update(updateDTO, context); }
                        catch (Exception ex)
                        {
                            _logger.LogError(LogMessages.ERRORWHENUPDATINGOBJECT,
                                             "Consistent Reading", ex.Message);
                        }

                        context.Commit();
                        continue;
                    }

                    var insertDTO = _consistentReadingInsertDTOFactory.
                    CreateConsistentReadingInsertDTO(creationResult.Value,
                                                     readingDTO.ReadingId);
                    insertDTOs.Add(insertDTO);

                    var successUpdateDTO = new ReadingUpdateDTO(readingDTO.ReadingId,
                                                    Status.Success, new List<string>());

                    try { _readingRepository.Update(successUpdateDTO, context); }
                    catch (Exception ex)
                    {
                        _logger.LogError(LogMessages.ERRORWHENUPDATINGOBJECT, 
                                         "Consistent Reading", ex.Message);
                    }

                    context.Commit();
                }
            }

            using (IContext insertContext = _contextFactory.Create())
            {
                if (insertDTOs.Count > 0)
                {
                    foreach (var insertDTO in insertDTOs)
                    {
                        try 
                        { 
                            _consistentReadingRepository.Insert(insertDTO, insertContext); 
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(LogMessages.ERRORWHENINSERTINGOBJECT, 
                                             "Consistent Reading", ex.Message);
                        }
                    }
                }

                insertContext.Commit();
            }
        }
    }
}
