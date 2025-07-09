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
        private readonly IReadingInsertDTOFactory _readingInsertDTOFactory;
        private readonly ILogger<ReadingService> _logger;

        public ReadingService(IContextFactory contextFactory,
                              IInputFileRepository inputFileRepository,
                              IReadingImportService readingImportService,
                              IReadingRepository readingRepository,
                              IReadingInsertDTOFactory readingInsertDTOFactory,
                              ILogger<ReadingService> logger)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
            _readingRepository = readingRepository;
            _readingImportService = readingImportService;
            _readingInsertDTOFactory = readingInsertDTOFactory;
            _logger = logger;
        }

        public void ProcessInputFiles()
        {
            using (IContext context = _contextFactory.Create())
            {
                var inputFiles = _inputFileRepository.GetByStatus(Status.New, context);

                if (inputFiles.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Input File");
                    return;
                }

                IEnumerable<ReadingInsertDTO> insertDTOs = null;

                foreach (var inputFile in inputFiles)
                {
                    using (var stream = new MemoryStream(inputFile.Bytes))
                    {
                        ImportResult importResult = _readingImportService.Import(stream);
                        if (importResult.Errors.Count > 0 && !importResult.Success)
                        {
                            if (importResult.Readings.Count == 0)
                            {
                                _logger.LogError(LogMessages.NOREADINGIMPORTED, inputFile.Name);
                                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                            }

                            var inputFileUpdate = new InputFileUpdateDTO(inputFile.InputFileId,
                                                                         Status.Error,
                                                                         importResult.Errors);
                            _inputFileRepository.Update(inputFileUpdate, context);
                        }

                        if (importResult.Readings.Any())
                            insertDTOs = _readingInsertDTOFactory.CreateReadingInsertDTOList
                                (importResult.Readings, inputFile.InputFileId);
                    }
                }

                foreach (var insertDTO in insertDTOs)
                    _readingRepository.Insert(insertDTO, context);
            }
        }
    }
}