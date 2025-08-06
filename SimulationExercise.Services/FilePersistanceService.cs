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
        private readonly IAverageProvinceDataService _averageProvinceDataService;
        private readonly IConsistentReadingExportService _consistentReadingExportService;
        private readonly IAverageProvinceDataExportService _averageProvinceDataExportService;

        public FilePersistanceService(IInputFileService inputFileService,
                                      IReadingService readingService,
                                      IConsistentReadingService consistentReadingService,
                                      IAverageProvinceDataService averageProvinceDataService,
                                      IConsistentReadingExportService consistentReadingExportService,
                                      IAverageProvinceDataExportService averageProvinceDataExportService)
        {
            _inputFileService = inputFileService;
            _readingService = readingService;
            _consistentReadingService = consistentReadingService;

            _averageProvinceDataService = averageProvinceDataService;
            _consistentReadingExportService = consistentReadingExportService;
            _averageProvinceDataExportService = averageProvinceDataExportService;
        }

        public void Initialize(string inDirectoryPath) =>
            _inputFileService.ProcessFiles(inDirectoryPath);

        public void CreateReadings() => 
            _readingService.ProcessInputFiles();

        public void CreateConsistentReadings() => 
            _consistentReadingService.ProcessReadings();

        public void CreateAverageProvinceDatas() => 
            _averageProvinceDataService.ProcessConsistentReadings();

        public void CreateAverageProvinceDataOutputFiles() =>
            _averageProvinceDataExportService.CreateOutputFiles();

        public void CreateConsistentReadingOutputFiles() =>
            _consistentReadingExportService.CreateOutputFiles();

        public void ExportAverageProvinceData(string outDirectoryPath) => 
            _averageProvinceDataExportService.Export(outDirectoryPath);

        public void ExportConsistentReadings(string outDirectoryPath) => 
            _consistentReadingExportService.Export(outDirectoryPath);

        public void LoggerConfiguration(string baseOutPath) =>
            LogPathHolder.ErrorLogPath = GetExportDirectoryPath(baseOutPath);

        private string GetExportDirectoryPath(string baseOutPath)
        {
            string exportDirectoryName = SystemTime.Now()
                .ToString("yyyyMMdd_HHmmss");

            string exportDirectoryPath = Path.Combine(baseOutPath, exportDirectoryName);
            string errorsFilePath = Path.Combine(exportDirectoryPath, "Errors.log");

            if (!Directory.Exists(exportDirectoryPath))
                Directory.CreateDirectory(exportDirectoryPath);

            return errorsFilePath;
        }
    }
}
