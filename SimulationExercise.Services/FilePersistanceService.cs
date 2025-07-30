using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Utilities;
using SimulationExercise.Services.Utilities;

namespace SimulationExercise.Services
{
    public class FilePersistanceService : IFilePersistanceService
    {
        private readonly IInputFileService _inputFileService;
        private readonly IReadingService _readingService;
        private readonly IConsistentReadingService _consistentReadingService;
        private readonly IOutputFileService _outputFileService;

        public FilePersistanceService(IInputFileService inputFileService,
                                      IReadingService readingService,
                                      IConsistentReadingService consistentReadingService,
                                      IOutputFileService outputFileService)
        {
            _inputFileService = inputFileService;
            _readingService = readingService;
            _consistentReadingService = consistentReadingService;
            _outputFileService = outputFileService;
        }

        public void Initialize(string inDirectoryPath) =>
            _inputFileService.ProcessFiles(inDirectoryPath);

        public void CreateReadings() => 
            _readingService.ProcessInputFiles();

        public void CreateConsistentReadings() => 
            _consistentReadingService.ProcessReadings();

        public void CreateOutputFiles() => 
            _outputFileService.ProcessConsistentReadings();

        public void LoggerConfiguration(string baseOutPath) =>
            LogPathHolder.ErrorLogPath = GetExportDirectoryPath(baseOutPath);

        private string GetExportDirectoryPath(string baseOutPath)
        {
            string errorDirectoryName = SystemTime.Now()
                .ToString("yyyyMMdd_HHmmss");

            string exportDirectoryPath = Path.Combine(baseOutPath, errorDirectoryName);
            string errorsFilePath = Path.Combine(exportDirectoryPath, "Errors.log");

            if (!Directory.Exists(exportDirectoryPath))
                Directory.CreateDirectory(exportDirectoryPath);

            return errorsFilePath;
        }
    }
}
