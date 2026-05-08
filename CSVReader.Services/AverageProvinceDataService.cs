using Microsoft.Extensions.Logging;
using CSVReader.Core.Common;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;

namespace CSVReader.Services
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

        public async Task ProcessConsistentReadingsAsync()
        {
            IList<ConsistentReadingGetDTO>? crGetDTOs = null;
            using (IContext searchContext = _contextFactory.Create())
                crGetDTOs = await _consistentReadingRepository.GetByIsExportedAsync(false, searchContext);

            if (crGetDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONONEXPORTEDOBJECTSFOUND, "ConsistentReading");
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
                                                              cr.IsHistoric, cr.DaysOfMeasure, 
                                                              cr.UtmNord, cr.UtmEst, 
                                                              cr.Latitude!, cr.Longitude!)).ToList();

                    var apdCreationResults = _averageProvinceDataFactory
                        .CreateAverageProvinceData(consistentReadings);

                    foreach(var apdCreationResult in apdCreationResults)
                    {
                        if (apdCreationResult.Success)
                        {
                            var apd = apdCreationResult.Value;
                            var insertDTO = new AverageProvinceDataInsertDTO(apd!.Province, apd.SensorTypeName,
                                                                             apd.AverageValue, apd.Unit,
                                                                             apd.AverageDaysOfMeasure, false);

                            await _averageProvinceDataRepository.InsertAsync(insertDTO, context);
                        }
                        else
                        {
                            _logger.LogError(LogMessages.ERRORSFOUND, "Average Province Data", _errorGroupNumber);
                            foreach (var error in apdCreationResult.Errors!) _logger.LogError(error);
                            _errorGroupNumber++;
                        }
                    }

                    context.Commit();
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
