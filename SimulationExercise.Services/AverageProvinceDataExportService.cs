using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOs.CSVDTOs;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Services
{
    public class AverageProvinceDataExportService : IAverageProvinceDataExportService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IAverageProvinceDataRepository _averageProvinceDataRepository;
        private readonly IAverageProvinceDataExportDTOFactory _averageProvinceDataExportDTOFactory;
        private readonly IOutputFileService _outputFileService;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly ILogger<AverageProvinceDataExportService> _logger;

        public AverageProvinceDataExportService(IContextFactory contextFactory,
                                                IAverageProvinceDataRepository averageProvinceDataRepository,
                                                IAverageProvinceDataExportDTOFactory averageProvinceDataExportDTOFactory,
                                                IOutputFileService outputFileService,
                                                IOutputFileRepository outputFileRepository,
                                                ILogger<AverageProvinceDataExportService> logger)
        {
            _contextFactory = contextFactory;
            _averageProvinceDataRepository = averageProvinceDataRepository;
            _averageProvinceDataExportDTOFactory = averageProvinceDataExportDTOFactory;
            _outputFileService = outputFileService;
            _outputFileRepository = outputFileRepository;
            _logger = logger;
        }

        public async Task CreateOutputFilesAsync()
        {
            IList<AverageProvinceDataGetDTO> apdGetDTOs;
            using (IContext searchContext = _contextFactory.Create())
                apdGetDTOs = await _averageProvinceDataRepository.GetByIsExportedAsync(false, searchContext);

            if (apdGetDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONONEXPORTEDOBJECTSFOUND, "Average Province Data");
                return;
            }

            try 
            {
                var records = _averageProvinceDataExportDTOFactory.CreateExportDTOList(apdGetDTOs);
                var result = _outputFileService.CreateOutputFilesAsync<AverageProvinceDataExportDTO>(records);

                if (!result.Result.Success)
                {
                    _logger.LogError(LogMessages.ERRORSFOUND, "Average Province Data", 0);
                    foreach (var error in result.Result.Errors!) _logger.LogError(error);
                }

                foreach (var averageProvinceData in apdGetDTOs)
                {
                    var updateDTO = new AverageProvinceDataUpdateDTO(averageProvinceData.AverageProvinceDataId, true);

                    using (IContext updateContext = _contextFactory.Create())
                    {
                        await _averageProvinceDataRepository.UpdateAsync(updateDTO, updateContext);
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
                IList<OutputFileGetDTO>? outputFiles = null;

                using (IContext searchContext = _contextFactory.Create())
                    outputFiles = await _outputFileRepository.GetByIsExportedAsync(false, "AverageProvinceData", searchContext);

                if (outputFiles.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Output File");
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