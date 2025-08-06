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
    public class AverageProvinceDataService : IAverageProvinceDataService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IAverageProvinceDataRepository _averageProvinceDataRepository;
        private readonly IAverageProvinceDataFactory _averageProvinceDataFactory;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private ILogger<AverageProvinceDataService> _logger;
        private int _errorGroupNumber = 0;

        public AverageProvinceDataService(IContextFactory contextFactory,
                                          IAverageProvinceDataRepository averageProvinceDataRepository,
                                          IAverageProvinceDataFactory averageProvinceDataFactory,
                                          IConsistentReadingRepository consistentReadingRepository,
                                          ILogger<AverageProvinceDataService> logger)
        {
            _contextFactory = contextFactory;
            _averageProvinceDataRepository = averageProvinceDataRepository;
            _averageProvinceDataFactory = averageProvinceDataFactory;
            _consistentReadingRepository = consistentReadingRepository;
            _logger = logger;
        }

        public void ProcessConsistentReadings()
        {
            IList<ConsistentReadingGetDTO>? crGetDTOs = null;
            using (IContext searchContext = _contextFactory.Create())
                crGetDTOs = _consistentReadingRepository.GetByStatus(Status.New, searchContext);

            if (crGetDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "OutputFile");
                return;
            }

            using (IContext context = _contextFactory.Create())
            {
                try
                {
                    var consistentReadings = crGetDTOs.Select(cr =>
                                        new ConsistentReading(cr.SensorId,
                                                              cr.SensorTypeName!,
                                                              cr.Unit, cr.Value,
                                                              cr.Province!, cr.City!,
                                                              cr.IsHistoric, cr.UtmNord,
                                                              cr.UtmEst, cr.Latitude!,
                                                              cr.Longitude!)).ToList();

                    var apdCreationResult = _averageProvinceDataFactory
                        .CreateAverageProvinceData(consistentReadings);

                    if (apdCreationResult.Success)
                    {
                        var apd = apdCreationResult.Value;
                        var insertDTO = new AverageProvinceDataInsertDTO(apd!.Province, apd.SensorTypeName,
                                                                         apd.AverageValue, apd.Unit,
                                                                         apd.AverageDaysOfMeasure, false);

                        _averageProvinceDataRepository.Insert(insertDTO, context);

                        foreach (var crGetDTO in crGetDTOs)
                        {
                            var updateDTO = new ConsistentReadingUpdateDTO(crGetDTO.ConsistentReadingId, 
                                                                           Status.Success, crGetDTO.IsExported);

                            _consistentReadingRepository.Update(updateDTO, context);
                        }
                    }
                    else
                    {
                        _logger.LogError(LogMessages.ERRORSFOUND, "Average Province Data", _errorGroupNumber);
                        foreach (var error in apdCreationResult.Errors!) _logger.LogError(error);
                        _errorGroupNumber++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
                    return;
                }
                finally
                {
                    context.Dispose();
                }
            }
        }
    }
}
