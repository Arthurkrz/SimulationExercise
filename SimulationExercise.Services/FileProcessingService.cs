using Microsoft.Extensions.Logging;
using Serilog;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using System.Runtime.CompilerServices;

namespace SimulationExercise.Services
{
    // separação em métodos privados
    // metodo generico para mensagem que recebe type e usa nameof(type)

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
            string exportFilePath = ExportDirectoryPathGeneratorAndLoggerConfiguration(baseOutDirectoryPath);
            int i = 0;

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
                    _logger.LogInformation("Importing readings...");
                    importResult = _readingImportService.Import(str);
                } 

                if (!importResult.Success)
                {
                    _logger.LogError($"Errors found! ({i++})");
                    foreach (var error in importResult.Errors)
                    {
                        _logger.LogError(error);
                        continue;
                    }
                }

                if (importResult.Readings.Count == 0)
                {
                    _logger.LogError("No readings have been imported!");
                    _logger.LogInformation("Continuing to next file (if exists)...\n");
                    continue;
                }

                _logger.LogInformation(SuccessMessageGenerator(typeof(Reading),
                                                  importResult.Readings.Count));

                IList<ConsistentReading> consistentReadings = 
                    new List<ConsistentReading>();

                _logger.LogInformation("Creating consistent readings...");
                foreach (Reading reading in importResult.Readings)
                {
                    var cr = _consistentReadingFactory.CreateConsistentReading(reading);
                    if (!cr.Success)
                    {
                        _logger.LogError($"Errors found! ({i++})");
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
                    _logger.LogInformation("Continuing to next file (if exists)...\n");
                    continue;
                }

                _logger.LogInformation(SuccessMessageGenerator(typeof(ConsistentReading), 
                                                               consistentReadings.Count));

                _logger.LogInformation("Creating province data...");
                IList<ProvinceData> provinceDatas =
                    _provinceDataListFactory.CreateProvinceDataList(consistentReadings); 

                if (provinceDatas.Count == 0)
                {
                    _logger.LogError("No province data have been created!");
                    _logger.LogInformation("Continuing to next file (if exists)...\n");
                    continue;
                }

                _logger.LogInformation(SuccessMessageGenerator(typeof(ProvinceData), 
                                                               provinceDatas.Count));

                IList<AverageProvinceData> averageProvinceDatas =
                    new List<AverageProvinceData>();

                _logger.LogInformation("Creating average province data...");
                foreach (ProvinceData provinceData in provinceDatas)
                {
                    var averageProvinceData = _averageProvinceDataFactory.CreateAverageProvinceData(provinceData);

                    if (!averageProvinceData.Success)
                    {
                        _logger.LogError($"Errors found! ({i++})");
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
                    _logger.LogInformation("Continuing to next file (if exists)...\n");
                    continue;
                }

                _logger.LogInformation(SuccessMessageGenerator(typeof(AverageProvinceData), 
                                                               averageProvinceDatas.Count));

                _logger.LogInformation("Exporting average province data...");
                using var fileStream = new FileStream(exportFilePath,
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

        private string SuccessMessageGenerator(Type type, int count)
        {
            string typeName = type.Name;

            return count == 1
                ? $"1 {typeName} created successfully!"
                : $"{count} {typeName}s created successfully!";
        }
    }
}
