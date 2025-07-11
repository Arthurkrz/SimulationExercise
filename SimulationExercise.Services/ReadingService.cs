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
                IEnumerable<ReadingInsertDTO> insertDTOs = null;
                var inputFiles = _inputFileRepository.GetByStatus(Status.New, context);

                if (inputFiles.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Input File");
                    return;
                }

                foreach (var inputFile in inputFiles)
                {
                    using (var stream = new MemoryStream(inputFile.Bytes))
                    {
                        ImportResult importResult = _readingImportService.Import(stream);

                        if (importResult.Readings.Any())
                            insertDTOs = _readingInsertDTOFactory.CreateReadingInsertDTOList
                                (importResult.Readings, inputFile.InputFileId);

                        if (importResult.Errors.Count > 0)
                        {
                            foreach (var error in importResult.Errors)
                                _logger.LogError(error);
                            
                            var inputFileUpdate = new InputFileUpdateDTO(inputFile.InputFileId,
                                                                         Status.Error,
                                                                         importResult.Errors);

                            try { _inputFileRepository.Update(inputFileUpdate, context); }
                            catch (Exception ex)
                            {
                                _logger.LogError(LogMessages.ERRORWHENUPDATINGOBJECT, 
                                                 "Input File", ex.Message);

                                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                            }
                        }
                    }
                }

                foreach (var insertDTO in insertDTOs)
                {
                    try { _readingRepository.Insert(insertDTO, context); }
                    catch (Exception ex)
                    {
                        _logger.LogError(LogMessages.ERRORWHENINSERTINGOBJECT, 
                                         "Reading", ex.Message);
                    }
                }
            }
        }
    }
}