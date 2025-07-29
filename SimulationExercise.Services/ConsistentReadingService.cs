using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Factories;
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
        private int _errorGroupNumber = 0;

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
            IList<ReadingGetDTO>? readingDTOs = null;
            using (IContext searchContext = _contextFactory.Create())
                readingDTOs = _readingRepository.GetByStatus(Status.New, searchContext);

            if (readingDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Reading");
                return;
            }

            foreach (var readingDTO in readingDTOs)
            {
                using (IContext context = _contextFactory.Create())
                {
                    try
                    {
                        var reading = ReadingGenerator(readingDTO);

                        var creationResult = _consistentReadingFactory
                            .CreateConsistentReading(reading);

                        if (creationResult.Success)
                        {
                            var insertDTO = _consistentReadingInsertDTOFactory.
                            CreateConsistentReadingInsertDTO(creationResult.Value!,
                                                             readingDTO.ReadingId);

                            var successDTO = new ReadingUpdateDTO(readingDTO.ReadingId, Status.Success);

                            _consistentReadingRepository.Insert(insertDTO, context);
                            _readingRepository.Update(successDTO, context);
                        }
                        else
                        {
                            _logger.LogError(LogMessages.ERRORSFOUND, "Reading", _errorGroupNumber);
                            foreach (var error in creationResult.Errors!) _logger.LogError(error);

                            var updateErrorDTO = new ReadingUpdateDTO(readingDTO.ReadingId,
                                                                        Status.Error,
                                                                        creationResult.Errors);

                            _readingRepository.Update(updateErrorDTO, context);
                            _errorGroupNumber++;
                        }

                        context.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
                    }
                    finally
                    {
                        context.Dispose();
                    }
                }
            }
        }

        private Reading ReadingGenerator(ReadingGetDTO readingDTO)
        {
            return new Reading(readingDTO.SensorId, readingDTO.SensorTypeName,
                               readingDTO.Unit, readingDTO.StationId,
                               readingDTO.StationName, readingDTO.Value,
                               readingDTO.Province, readingDTO.City,
                               readingDTO.IsHistoric, readingDTO.StartDate,
                               readingDTO.StopDate, readingDTO.UtmNord,
                               readingDTO.UtmEst, readingDTO.Latitude,
                               readingDTO.Longitude);
        }
    }
}