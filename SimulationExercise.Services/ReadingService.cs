using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadingImportService _readingImportService;
        private readonly ILogger<ReadingService> _logger;

        public ReadingService(IContextFactory contextFactory,
                              IInputFileRepository inputFileRepository,
                              IReadingImportService readingImportService,
                              IReadingRepository readingRepository,
                              ILogger<ReadingService> logger)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
            _readingRepository = readingRepository;
            _readingImportService = readingImportService;
            _logger = logger;
        }

        public void ProcessInputFiles()
        {
            using (IContext context = _contextFactory.Create())
            {
                var inputFiles = _inputFileRepository.GetByStatus(Status.New, context);

                if (inputFiles.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND);
                    return;
                }

                IEnumerable<ReadingInsertDTO> insertDTOs = null;

                foreach (var inputFile in inputFiles)
                {
                    using (var stream = new MemoryStream(inputFile.Bytes))
                    {
                        ImportResult importResult = _readingImportService.Import(stream);

                        if (!importResult.Success && importResult.Readings.Count == 0)
                        {
                            _logger.LogError(LogMessages.NOREADINGIMPORTED);
                            _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                            continue;
                        }

                        if (importResult.Errors.Count > 0)
                        {
                            var inputFileUpdate = new InputFileUpdateDTO(inputFile.InputFileId,
                                                                         Status.Error,
                                                                         importResult.Errors);
                            _inputFileRepository.Update(inputFileUpdate, context);
                        }

                        if (importResult.Readings.Any())
                            insertDTOs = importResult.Readings.Select(r => new ReadingInsertDTO(
                                            inputFile.InputFileId, r.SensorId, r.SensorTypeName,
                                            r.Unit, r.StationId, r.StationName, r.Value,
                                            r.Province, r.City, r.IsHistoric, r.StartDate,
                                            r.StopDate, r.UtmNord, r.UtmEst, r.Latitude,
                                            r.Longitude, Status.Success));
                    }
                }

                foreach (var insertDTO in insertDTOs)
                    _readingRepository.Insert(insertDTO, context);
            }
        }
    }
}