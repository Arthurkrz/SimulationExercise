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

        public async Task Initialize(string inDirectoryPath) =>
            await _inputFileService.ProcessFilesAsync(inDirectoryPath);

        public async Task CreateReadings() => 
           await _readingService.ProcessInputFilesAsync();

        public async Task CreateConsistentReadings() => 
            await _consistentReadingService.ProcessReadingsAsync();

        public async Task CreateAverageProvinceDatas() => 
            await _averageProvinceDataService.ProcessConsistentReadingsAsync();

        public async Task CreateAverageProvinceDataOutputFiles() =>
            await _averageProvinceDataExportService.CreateOutputFilesAsync();

        public async Task CreateConsistentReadingOutputFiles() =>
            await _consistentReadingExportService.CreateOutputFilesAsync();

        public async Task ExportAverageProvinceData(string outDirectoryPath) => 
            await _averageProvinceDataExportService.ExportAsync(outDirectoryPath);

        public async Task ExportConsistentReadings(string outDirectoryPath) => 
            await _consistentReadingExportService.ExportAsync(outDirectoryPath);

        public bool LoggerConfiguration(string baseOutPath)
        {
            try { LogPathHolder.ErrorLogPath = GetExportDirectoryPath(baseOutPath); }
            catch (Exception) { return false; }

            return true;
        }

        private string GetExportDirectoryPath(string baseOutPath)
        {
            if (!Directory.Exists(baseOutPath))
                throw new InvalidOperationException("Path not located.");

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
