using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using System.Text;
using System.Text.Json;

namespace SimulationExercise.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadingImportService _readingImportService;
        private readonly IConsistentReadingService _consistentReadingService;
        private readonly ILogger<ReadingService> _logger;

        public ReadingService(IContextFactory contextFactory,
                              IInputFileRepository inputFileRepository,
                              IReadingImportService readingImportService,
                              IReadingRepository readingRepository,
                              IConsistentReadingService consistentReadingService,
                              ILogger<ReadingService> logger)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
            _readingRepository = readingRepository;
            _readingImportService = readingImportService;
            _consistentReadingService = consistentReadingService;
            _logger = logger;
        }

        public void ProcessInputFiles(InputFileGetDTO inputFile)
        {
            using (IContext context = _contextFactory.Create())
            {
                List<ReadingInsertDTO> inserts = new List<ReadingInsertDTO>();

                using (var stream = new MemoryStream(inputFile.Bytes))
                {
                    ImportResult importResult = _readingImportService.Import(stream);

                    if (importResult.Errors.Count > 0)
                    {
                        var inputFileUpdate = new InputFileUpdateDTO(inputFile.InputFileId, 
                                                                     Status.Error, 
                                                                     importResult.Errors);

                        _inputFileRepository.Update(inputFileUpdate, context);
                    }

                    if (importResult.Readings.Any())
                    {
                        foreach (var reading in importResult.Readings)
                        {
                            string readingJson = JsonSerializer.Serialize(reading);
                            byte[] readingBytes = Encoding.UTF8.GetBytes(readingJson);

                            var readingInsertDTO = new ReadingInsertDTO(inputFile.InputFileId, 
                                                                        readingBytes, 
                                                                        Status.New);
                            inserts.Add(readingInsertDTO);
                            continue;
                        }
                    }
                }

                if (inserts.Count > 0)
                {
                    foreach (var insert in inserts)
                        _readingRepository.Insert(insert, context);

                    var readings = _readingRepository.GetByStatus(Status.New, context);
                    _consistentReadingService.ProcessReadings(readings);
                }

                _logger.LogError(LogMessages.NOREADINGIMPORTED);
                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
            }
        }
    }
}