using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.CSVDTOs;
using SimulationExercise.Core.DTOS;

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

        public void CreateOutputFiles()
        {
            IList<ConsistentReadingGetDTO> crGetDTOs;
            using (IContext searchContext = _contextFactory.Create())
                crGetDTOs = _consistentReadingRepository.GetByIsExported(false, searchContext);

            if (crGetDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Consistent Reading");
                return;
            }

            try
            {
                var records = _consistentReadingExportDTOFactory.CreateExportDTOList(crGetDTOs);
                var result = _outputFileService.CreateOutputFiles<ConsistentReadingExportDTO>(records);
                
                if (!result.Success)
                {
                    _logger.LogError(LogMessages.ERRORSFOUND, "Consistent Reading list", 0);
                    foreach (var error in result.Errors!) _logger.LogError(error);
                }

                foreach (var consistentReading in crGetDTOs)
                {
                    var updateDTO = new ConsistentReadingUpdateDTO(consistentReading.ConsistentReadingId, 
                                                                   consistentReading.Status, true);

                    using (IContext updateContext = _contextFactory.Create())
                    {
                        _consistentReadingRepository.Update(updateDTO, updateContext);
                        updateContext.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
            }
        }

        public void Export(string outDirectoryPath)
        {
            IList<OutputFileGetDTO>? outputFiles = null;

            try
            {
                using (IContext searchContext = _contextFactory.Create())
                    outputFiles = _outputFileRepository.GetByIsExported(false, searchContext);

                if (outputFiles.Count == 0)
                {
                    _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Output File");
                    return;
                }

                var fileStream = new FileStream(outDirectoryPath,
                                                FileMode.Create,
                                                FileAccess.Write);

                foreach (var outputFile in outputFiles)
                    _outputFileService.Export<ConsistentReadingExportDTO>(outputFile, fileStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
            }
        }
    }
}