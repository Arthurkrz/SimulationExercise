using FileHelpers;
using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using System.Text;

namespace SimulationExercise.Services
{
    public class ConsistentReadingExportService : IConsistentReadingExportService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private readonly IConsistentReadingExportDTOFactory _consistentReadingExportDTOFactory;
        private readonly IOutputFileService _outputFileService;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly ILogger<ConsistentReadingExportService> _logger;

        public ConsistentReadingExportService(IContextFactory contextFactory,
                                              IConsistentReadingRepository consistentReadingRepository,
                                              IConsistentReadingExportDTOFactory consistentReadingExportDTOFactory,
                                              IOutputFileService outputFileService,
                                              IOutputFileRepository outputFileRepository,
                                              ILogger<ConsistentReadingExportService> logger)
        {
            _contextFactory = contextFactory;
            _consistentReadingRepository = consistentReadingRepository;
            _consistentReadingExportDTOFactory = consistentReadingExportDTOFactory;
            _outputFileService = outputFileService;
            _outputFileRepository = outputFileRepository;
            _logger = logger;
        }

        public async Task CreateOutputFilesAsync()
        {
            IList<ConsistentReadingGetDTO> crGetDTOs;
            using (IContext searchContext = _contextFactory.Create())
                crGetDTOs = await _consistentReadingRepository.GetByIsExportedAsync(false, searchContext);

            if (crGetDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Consistent Reading");
                return;
            }

            try
            {
                var records = _consistentReadingExportDTOFactory.CreateExportDTOList(crGetDTOs);
                var result = _outputFileService.CreateOutputFilesAsync<ConsistentReadingExportDTO>(records);

                //if (!result.Success)
                //{
                //    _logger.LogError(LogMessages.ERRORSFOUND, "Consistent Reading list", 0);
                //    foreach (var error in result.Errors!) _logger.LogError(error);
                //}

                foreach (var consistentReading in crGetDTOs)
                {
                    var updateDTO = new ConsistentReadingUpdateDTO(consistentReading.ConsistentReadingId, true);

                    using (IContext updateContext = _contextFactory.Create())
                    {
                        await _consistentReadingRepository.UpdateAsync(updateDTO, updateContext);
                        updateContext.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
            }
        }

        public async Task ExportAsync(string outDirectoryPath)
        {
            try
            {
                IList<OutputFileGetDTO>? outputFiles;

                using (IContext searchContext = _contextFactory.Create())
                    outputFiles = await _outputFileRepository.GetByIsExportedAsync(false, "ConsistentReading", searchContext);

                if (outputFiles.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Output File");
                    return;
                }

                Directory.CreateDirectory(outDirectoryPath);

                var engine = new FileHelperEngine<ConsistentReadingExportDTO>();

                foreach (var outputFile in outputFiles)
                {
                    var fileName = $"{outputFile.Name}.csv";
                    var fullPath = Path.Combine(outDirectoryPath, fileName);
                    var records = BuildRecords(outputFile.Bytes, engine);

                    using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    using var streamWriter = new StreamWriter(fileStream, leaveOpen: true);

                    engine.WriteStream(streamWriter, records);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
            }
        }

        private ConsistentReadingExportDTO[] BuildRecords(byte[] bytes, FileHelperEngine<ConsistentReadingExportDTO> engine)
        {
            var text = Encoding.UTF8.GetString(bytes);
            text = text.Replace(',', '.');

            var normalizedBytes = Encoding.UTF8.GetBytes(text);

            using var memoryStream = new MemoryStream(normalizedBytes);
            using var streamReader = new StreamReader(memoryStream);
            return engine.ReadStream(streamReader);
        }
    }
}