using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IReadingImportService _readingImportService;
        private readonly IConsistentReadingFactory _consistentReadingFactory;
        private readonly IProvinceDataListFactory _provinceDataListFactory;
        private readonly IAverageProvinceDataFactory _averageProvinceDataFactory;
        private readonly IAverageProvinceDataExportService _averageProvinceDataExportService;
        private readonly ILogger<FileProcessingService> _logger;

        public FileProcessingService(IReadingImportService readingImportService,
                                     IConsistentReadingFactory consistentReadingFactory,
                                     IProvinceDataListFactory provinceDataListFactory,
                                     IAverageProvinceDataFactory averageProvinceDataFactory,
                                     IAverageProvinceDataExportService averageProvinceDataExportService,
                                     ILogger<FileProcessingService> logger)
        {
            _readingImportService = readingImportService;
            _consistentReadingFactory = consistentReadingFactory;
            _provinceDataListFactory = provinceDataListFactory;
            _averageProvinceDataFactory = averageProvinceDataFactory;
            _averageProvinceDataExportService = averageProvinceDataExportService;
            _logger = logger;
        }

        public void ProcessFile(string inDirectoryPath, string baseOutDirectoryPath)
        {
            string noErrorsFilePath = ExportDirectoryPathGeneratorAndLoggerConfiguration(baseOutDirectoryPath);

            if (!Directory.Exists(inDirectoryPath))
            {
                Directory.CreateDirectory(inDirectoryPath);
            }

            var files = Directory.GetFiles(inDirectoryPath);
            if (files.Length == 0)
            {
                _logger.LogError("No CSV files found in the 'IN' directory.");
                return;    
            }

            foreach (var file in files)
            {
                ImportResult importResult = null;
                using(var str = new FileStream(file,
                                                FileMode.Open,
                                                FileAccess.Read))
                {
                    importResult = _readingImportService.Import(str);
                } 

                if (!importResult.Success)
                {
                    _logger.LogError("Errors found!");
                    foreach (var error in importResult.Errors)
                    {
                        _logger.LogError(error);
                        continue;
                    }
                }

                if (importResult.Readings.Count == 0)
                {
                    _logger.LogError("No readings have been imported!");
                    throw new Exception("No readings have been imported!");
                }

                string successMessageImport = importResult.Readings.Count == 1
                ? "1 reading imported successfully!"
                : $"{importResult.Readings.Count} readings imported successfully!";
                _logger.LogInformation(successMessageImport);

                IList<ConsistentReading> consistentReadings = 
                    new List<ConsistentReading>();

                foreach (Reading reading in importResult.Readings)
                {
                    var cr = _consistentReadingFactory.CreateConsistentReading(reading);
                    if (!cr.Success)
                    {
                        _logger.LogError($"Errors found!");
                        foreach (var error in cr.Errors)
                        {
                            _logger.LogError(error);
                            continue;
                        }
                    }

                    else consistentReadings.Add(cr.Value);
                }

                if (consistentReadings.Count == 0)
                {
                    _logger.LogError("No consistent readings have been created!");
                    throw new Exception("No consistent readings have been created!");
                }

                string successMessageConsistentReadingCreation = consistentReadings.Count == 1
                ? "1 consistent reading created successfully!"
                : $"{consistentReadings.Count} consistent readings created successfully!";
                _logger.LogInformation(successMessageConsistentReadingCreation);

                IList<ProvinceData> provinceDatas =
                    _provinceDataListFactory.CreateProvinceDataList(consistentReadings); 

                if (provinceDatas.Count == 0)
                {
                    _logger.LogError("No province data have been created!");
                    throw new Exception("No province data have been created!");
                }

                string successMessageProvinceDataCreation = provinceDatas.Count == 1
                ? "1 province data created successfully!"
                : $"{provinceDatas.Count} province datas created successfully!";
                _logger.LogInformation(successMessageProvinceDataCreation);

                IList<AverageProvinceData> averageProvinceDatas =
                    new List<AverageProvinceData>();

                foreach (ProvinceData provinceData in provinceDatas)
                {
                    var averageProvinceData = _averageProvinceDataFactory.CreateAverageProvinceData(provinceData);

                    if (!averageProvinceData.Success)
                    {
                        _logger.LogError("Errors found!");
                        foreach (var errors in averageProvinceData.Errors)
                        {
                            _logger.LogError(errors);
                            continue;
                        }
                    }

                    else averageProvinceDatas.Add(averageProvinceData.Value);
                }

                if (averageProvinceDatas.Count == 0)
                {
                    _logger.LogError("No average province data have been created!");
                    throw new Exception("No average province data have been created!");
                }

                string successMessageAverageProvinceDataCreation = averageProvinceDatas.Count == 1
                ? "1 average province data created successfully!"
                : $"{averageProvinceDatas.Count} average province datas created successfully!";
                _logger.LogInformation(successMessageAverageProvinceDataCreation);

                using var fileStream = new FileStream(noErrorsFilePath,
                                                        FileMode.Create,
                                                        FileAccess.Write);

                Log.CloseAndFlush();

                _averageProvinceDataExportService.Export(averageProvinceDatas,
                                                            fileStream);
            }
        }

        private string ExportDirectoryPathGeneratorAndLoggerConfiguration(string baseOutPath)
        {
            string specificReadingsAndErrorsDirectoryName = DateTime.Now.ToString
                                                    ("yyyyMMdd_HHmmss");

            string fullFolderPath = Path.Combine(baseOutPath,
                            specificReadingsAndErrorsDirectoryName);

            Directory.CreateDirectory(fullFolderPath);

            string noErrorsFilePath = Path.Combine(fullFolderPath, "AverageProvinceData.csv");
            string errorsFilePath = Path.Combine(fullFolderPath, "Errors.log");

            LogPathHolder.ErrorLogPath = errorsFilePath;

            return noErrorsFilePath;
        }
    }
}
