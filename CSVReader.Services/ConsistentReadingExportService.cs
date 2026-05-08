using Microsoft.Extensions.Logging;
using CSVReader.Core.Common;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.DTOs.CSVDTOs;
using CSVReader.Core.DTOs.DatabaseDTOs;

namespace CSVReader.Services
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
            try
            {
                IList<ConsistentReadingGetDTO> crGetDTOs;
                using (IContext searchContext = _contextFactory.Create())
                    crGetDTOs = await _consistentReadingRepository.GetByIsExportedAsync(false, searchContext);

                if (crGetDTOs.Count == 0)
                {
                    _logger.LogError(LogMessages.NONONEXPORTEDOBJECTSFOUND, "Consistent Reading");
                    return;
                }

                var records = _consistentReadingExportDTOFactory.CreateExportDTOList(crGetDTOs);
                var result = await _outputFileService.CreateOutputFilesAsync(records);

                if (!result.Success)
                {
                    _logger.LogError(LogMessages.ERRORSFOUND, "Consistent Reading list", 0);
                    foreach (var error in result.Errors!) _logger.LogError(error);
                }

                else
                {
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
                    _logger.LogError(LogMessages.NONONEXPORTEDOBJECTSFOUND, "Output File");
                    return;
                }

                Directory.CreateDirectory(outDirectoryPath);

                foreach (var outputFile in outputFiles)
                {
                    var fileName = $"{outputFile.Name}.csv";
                    var fullPath = Path.Combine(outDirectoryPath, fileName);

                    await File.WriteAllBytesAsync(fullPath, outputFile.Bytes);

                    var updateDTO = new OutputFileUpdateDTO(outputFile.OutputFileId, true);
                    using (IContext updateContext = _contextFactory.Create())
                    {
                        await _outputFileRepository.UpdateAsync(updateDTO, updateContext);
                        updateContext.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
            }
        }
    }
}