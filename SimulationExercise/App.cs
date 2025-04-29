using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using System;

namespace SimulationExercise.Console
{
    public class App
    {
        private readonly IReadingImportService _readingImportService;
        private readonly IConsistentReadingFactory _consistentReadingFactory;
        private readonly IProvinceDataListFactory _provinceDataListFactory;
        private readonly IAverageProvinceDataFactory _averageProvinceDataFactory;
        private readonly IAverageProvinceDataExportService _averageProvinceDataExportService;
        private readonly ILogger<App> _logger;

        public App(IReadingImportService readingImportService,
                   IConsistentReadingFactory consistentReadingFactory, 
                   IProvinceDataListFactory provinceDataListFactory,
                   IAverageProvinceDataFactory averageProvinceDataFactory,
                   IAverageProvinceDataExportService averageProvinceDataExportService,
                   ILogger<App> logger)
        {
            _readingImportService = readingImportService;
            _consistentReadingFactory = consistentReadingFactory;
            _provinceDataListFactory = provinceDataListFactory;
            _averageProvinceDataFactory = averageProvinceDataFactory;
            _averageProvinceDataExportService = averageProvinceDataExportService;
            _logger = logger;
        }

        public ImportResult ImportReadings(string inDirectoryPath)
        {
            if (string.IsNullOrEmpty(inDirectoryPath))
            {
                _logger.LogError("Empty 'IN' directory path (empty argument).");
                throw new ArgumentNullException(nameof(inDirectoryPath));
            }

            _logger.LogInformation("Locating entry point...\n");

            if (Directory.Exists(inDirectoryPath))
            {
                var files = Directory.GetFiles(inDirectoryPath, "*.csv");
                if (files.Length == 0)
                {
                    _logger.LogError("No CSV files found in the 'IN' directory.");
                    throw new FileNotFoundException(nameof(inDirectoryPath));
                }

                _logger.LogInformation("Entry point located!\n");

                using var stream = new FileStream(files[0], 
                                                  FileMode.Open, 
                                                  FileAccess.Read);
                return _readingImportService.Import(stream);
            }

            _logger.LogError("No 'IN' directory found in given path.");
            throw new DirectoryNotFoundException(nameof(inDirectoryPath));
        }

        public IList<Result<ConsistentReading>> CreateConsistentReadings(IList<Reading> readings)
        {
            IList<Result<ConsistentReading>> consistentReadings = new List<Result<ConsistentReading>>();
            if (readings == null)
            {
                _logger.LogError("Null IList<Reading> readings (argument).");
                throw new ArgumentNullException(nameof(readings));
            }

            if (readings.Count == 0)
            {
                _logger.LogError("No readings found in Reading list.");
                throw new ArgumentException(nameof(readings));
            }

            string message = readings.Count == 1
                ? "1 reading found in list.\n"
                : $"{readings.Count} readings found in list.\n";

            _logger.LogInformation(message);
            _logger.LogInformation("Creating consistent readings...\n");

            
            foreach (Reading reading in readings)
            {
                var consistentReading = _consistentReadingFactory.CreateConsistentReading(reading);
                consistentReadings.Add(consistentReading);
            }

            return consistentReadings;
        }

        public IList<ProvinceData> CreateProvinceDataList(IList<ConsistentReading> consistentReadings)
        {
            IList<ProvinceData> provinceDatas = new List<ProvinceData>();
            if (consistentReadings == null)
            {
                _logger.LogError("Null IList<ConsistentReading> consistentReadings (argument).");
                throw new ArgumentNullException(nameof(consistentReadings));
            }

            if (consistentReadings.Count == 0)
            {
                _logger.LogError("No consistent readings found in list.");
                throw new ArgumentException(nameof(consistentReadings));
            }

            string message = consistentReadings.Count == 1
                ? "1 consistent reading found in list.\n"
                : $"{consistentReadings.Count} consistent readings found in list.\n";
            _logger.LogInformation(message);
            _logger.LogInformation("Creating province data list...\n");

            return _provinceDataListFactory.CreateProvinceDataList(consistentReadings);
        }

        public IList<Result<AverageProvinceData>> CreateAverageProvinceData(IList<ProvinceData> provinceDatas)
        {
            IList<Result<AverageProvinceData>> averageProvinceDatas =
                new List<Result<AverageProvinceData>>();
            if (provinceDatas == null)
            {
                _logger.LogError("Null IList<ProvinceData> provinceDatas (argument).");
                throw new ArgumentNullException(nameof(provinceDatas));
            }

            if (provinceDatas.Count == 0)
            {
                _logger.LogError("No province data found in list.");
                throw new ArgumentException(nameof(provinceDatas));
            }

            string message = provinceDatas.Count == 1
                ? "1 province data found in list.\n"
                : $"{provinceDatas.Count} province data found in list.\n";
            _logger.LogInformation(message);
            _logger.LogInformation("Creating average province data...\n");

            foreach (ProvinceData provinceData in provinceDatas)
            {
                var averageProvinceData = 
                    _averageProvinceDataFactory.CreateAverageProvinceData(provinceData);

                averageProvinceDatas.Add(averageProvinceData);
            }

            return averageProvinceDatas;
        }

        public void ExportAverageProvinceData(string outDirectoryPath, 
                                              DateTime exportDate, 
                                              IList<AverageProvinceData> averageProvinceDatas)
        {
            if (averageProvinceDatas == null)
            {
                _logger.LogError("Null IList<AverageProvinceData> averageProvinceDatas (argument).");
                throw new ArgumentNullException(nameof(averageProvinceDatas));
            }

            if (averageProvinceDatas.Count == 0)
            {
                _logger.LogError("No average province data found in list.");
                throw new ArgumentException(nameof(averageProvinceDatas));
            }

            if (string.IsNullOrEmpty(outDirectoryPath))
            {
                _logger.LogError("Empty 'OUT' directory path (empty argument).");
                throw new ArgumentNullException(nameof(outDirectoryPath));
            }

            if (exportDate == default)
            {
                _logger.LogError("Exportation DateTime not given (argument).");
                throw new ArgumentNullException(nameof(exportDate));
            }

            if (string.IsNullOrEmpty(outDirectoryPath))
            {
                _logger.LogError("Empty 'OUT' directory path (empty argument).");
                throw new ArgumentNullException(nameof(outDirectoryPath));
            }

            string message = averageProvinceDatas.Count == 1
                ? "1 average province data found in list.\n"
                : $"{averageProvinceDatas.Count} average province data found in list.\n";
            _logger.LogInformation(message);

            _logger.LogInformation("Locating out point...\n");

            if (!Directory.Exists(outDirectoryPath))
            {
                _logger.LogError("No 'OUT' directory found in given path.");
                throw new DirectoryNotFoundException(nameof(outDirectoryPath));
            }

            _logger.LogInformation("Out point located!\n");

            string specificReadingsAndErrorsDirectoryName = exportDate
                                                            .ToString
                                                            ("yyyyMMdd_HHmmss");
            string fullFolderPath = Path.Combine(outDirectoryPath, 
                           specificReadingsAndErrorsDirectoryName);

            string filePath = Path.Combine(fullFolderPath, 
                                           "AverageProvinceData.csv");

            Directory.CreateDirectory(fullFolderPath);

            _logger.LogInformation($"Export folder named '{specificReadingsAndErrorsDirectoryName}' " +
                                   $"in path '{fullFolderPath}' created!\n");

            _logger.LogInformation("Exporting average province data to CSV...\n");

            using var fileStream = new FileStream(filePath, 
                                                  FileMode.Create, 
                                                  FileAccess.Write);

            _averageProvinceDataExportService.Export(averageProvinceDatas, 
                                                     fileStream);
        }

        //public void ExportErrors(IDictionary<ErrorType, IList<string>> errorsWithTypes)
        //{
        //    string[] errorFileName = errorsWithTypes.Keys.Select(key => key.ToString()).ToArray();
        //    errorsWithTypes.Values.
        //}
    }
}