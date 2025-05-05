using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using System.Reflection.Metadata;

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

        public void ProcessFile(string inDirectoryPath, string outDirectoryPath)
        {
            //Vericar se o Diretorio NAO existe se for caso cria
            if (Directory.Exists(inDirectoryPath))
            {
                _logger.LogInformation("Entry point located!\n");

                var files = Directory.GetFiles(inDirectoryPath);
                if (files.Length == 0)
                    _logger.LogError("No CSV files found in the 'IN' directory.");

                foreach (var file in files)
                {
                    using var stream = new FileStream(file,
                                                    FileMode.Open,
                                                    FileAccess.Read);
                    ImportResult importResult = _readingImportService.Import(stream);

                    using(var str = new FileStream(file,
                                                    FileMode.Open,
                                                    FileAccess.Read))
                    {
                        importResult = _readingImportService.Import(str);
                    } 

                    if (importResult.Readings.Count == 0)
                    {
                        _logger.LogError("No readings have been imported!");
                        throw new Exception("No readings have been imported!");
                    }

                    if (importResult.Success == false)
                        _logger.LogWarning("Errors ocurred while importing readings! " +
                                           "Check ErrorLog file for more information.\n");

                    string successMessageImport = importResult.Readings.Count == 1
                    ? "1 reading imported successfully!"
                    : $"{importResult.Readings.Count} readings imported successfully!";
                    _logger.LogInformation(successMessageImport);

                    IList<Result<ConsistentReading>> consistentReadingResults =
                        new List<Result<ConsistentReading>>();

                    foreach (Reading reading in importResult.Readings)
                    {
                        //meu jeito
                        var cr = _consistentReadingFactory.CreateConsistentReading(reading);
                        if (!cr.Success)
                        {
                            _logger.LogError($"Errors foundesd");
                            foreach (var error in cr.Errors)
                            {
                                _logger.LogError(error);
                            }

                            continue;
                        }

                        consistentReadingResults.Add(_consistentReadingFactory
                                                      .CreateConsistentReading(reading));
                    }

                    if (consistentReadingResults.All(cr => cr.Success == false))
                    {
                        _logger.LogError("No consistent readings have been created!");
                        throw new Exception("No consistent readings have been created!");
                    }

                    if (consistentReadingResults.Any(cr => cr.Success == false))
                        _logger.LogWarning("Errors ocurred while creating consistent readings! " +
                                           "Check ErrorLog file for more information.\n");

                    var consistentReadings = consistentReadingResults
                        .Where(cr => cr.Success)
                        .Select(cr => cr.Value)
                        .ToList();

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

                    IList<Result<AverageProvinceData>> averageProvinceDatasResult =
                        new List<Result<AverageProvinceData>>();

                    foreach (ProvinceData provinceData in provinceDatas)
                    {
                        averageProvinceDatasResult.Add(_averageProvinceDataFactory
                                                  .CreateAverageProvinceData
                                                             (provinceData));
                    }

                    var averageProvinceDatas = averageProvinceDatasResult
                        .Where(apd => apd.Success)
                        .Select(apd => apd.Value)
                        .ToList();

                    if (averageProvinceDatas.Count == 0)
                    {
                        _logger.LogError("No average province data have been created!");
                        throw new Exception("No average province data have been created!");
                    }

                    string successMessageAverageProvinceDataCreation = averageProvinceDatas.Count == 1
                    ? "1 average province data created successfully!"
                    : $"{averageProvinceDatas.Count} average province datas created successfully!";
                    _logger.LogInformation(successMessageAverageProvinceDataCreation);

                    _averageProvinceDataExportService.Export(averageProvinceDatas, stream);

                    string specificReadingsAndErrorsDirectoryName = DateTime.Now.ToString
                                                                      ("yyyyMMdd_HHmmss");

                    string fullFolderPath = Path.Combine(outDirectoryPath,
                                    specificReadingsAndErrorsDirectoryName);

                    string noErrorsFilePath = Path.Combine(fullFolderPath, "AverageProvinceData.csv");

                    Directory.CreateDirectory(fullFolderPath);

                    _logger.LogInformation($"Export folder named '{specificReadingsAndErrorsDirectoryName}' " +
                                            $"in path '{fullFolderPath}' created!\n");

                    using var fileStream = new FileStream(noErrorsFilePath,
                                                          FileMode.Create,
                                                          FileAccess.Write);

                    _averageProvinceDataExportService.Export(averageProvinceDatas,
                                                                fileStream);
                }
            }

            _logger.LogError("No 'IN' directory found in given path.");
            throw new Exception("No 'IN' directory found in given path.");
        }
    }
}
