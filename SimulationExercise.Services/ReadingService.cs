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
    public class ReadingService : IReadingService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadingImportService _readingImportService;
        private readonly IReadingInsertDTOFactory _readingInsertDTOFactory;
        private readonly ILogger<ReadingService> _logger;
        private int _errorGroupNumber = 0;

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
            IList<InputFileGetDTO>? inputFiles = null;
            using (IContext searchContext = _contextFactory.Create())
                inputFiles = _inputFileRepository.GetByStatus(Status.New, searchContext);

            if (inputFiles.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Input File");
                return;
            }

            foreach (var inputFile in inputFiles)
            {
                using (IContext context = _contextFactory.Create())
                {
                    try
                    {
                        ImportResult? importResult = null;

                        using (var stream = new MemoryStream(inputFile.Bytes))
                            importResult = _readingImportService.Import(stream);

                        if (!importResult.Success)
                        {
                            _logger.LogError(LogMessages.ERRORSFOUND, "Input File", _errorGroupNumber);
                            foreach (var error in importResult.Errors) _logger.LogError(error);

                            var inputFileUpdate = new InputFileUpdateDTO(inputFile.InputFileId,
                                                                         Status.Error,
                                                                         importResult.Errors);

                            _inputFileRepository.Update(inputFileUpdate, context);
                            _errorGroupNumber++;
                        }
                        else
                        {
                            var inputFileUpdate = new InputFileUpdateDTO(inputFile.InputFileId,
                                                                         Status.Success);

                            _inputFileRepository.Update(inputFileUpdate, context);
                        }

                        if (importResult.Readings.Any())
                        {
                            var insertDTOs = _readingInsertDTOFactory.CreateReadingInsertDTOList
                                                   (importResult.Readings, inputFile.InputFileId);

                            foreach (var insertDTO in insertDTOs)
                                _readingRepository.Insert(insertDTO, context);
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
    }
}