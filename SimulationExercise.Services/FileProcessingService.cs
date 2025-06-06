﻿using Microsoft.Extensions.Logging;
using Serilog;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Utilities;

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
        private int _errorGroupNumber;

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
            _errorGroupNumber = 0;

            if (!Directory.Exists(inDirectoryPath))
                Directory.CreateDirectory(inDirectoryPath);

            var files = Directory.GetFiles(inDirectoryPath);
            if (files.Length == 0)
            {
                _logger.LogError(LogMessages.NOCSVFILESFOUND);
                return;    
            }

            foreach (var file in files)
            {
                var importReadingsResult = ImportReadings(file);
                if (importReadingsResult.Readings.Count == 0) continue;

                var consistentReadingsResult = 
                    CreateConsistentReadings(importReadingsResult);
                if (consistentReadingsResult.Count == 0) continue;

                var createProvinceDataResult = 
                    CreateProvinceData(consistentReadingsResult);
                if (createProvinceDataResult.Count == 0) continue;

                var createAverageProvinceDataResult = 
                    CreateAverageProvinceData(createProvinceDataResult);
                if (createAverageProvinceDataResult.Count == 0) continue;

                ExportAverageProvinceData(noErrorsFilePath, 
                    createAverageProvinceDataResult);
            }
        }

        private ImportResult ImportReadings(string file)
        {
            ImportResult importResult = null;
            using (var str = new FileStream(file,
                                            FileMode.Open,
                                            FileAccess.Read))
            {
                _logger.LogInformation(LogMessages.IMPORTREADING);
                importResult = _readingImportService.Import(str);
            }

            if (!importResult.Success)
            {
                _logger.LogError(LogMessages.ERRORSFOUND, _errorGroupNumber);
                foreach (var error in importResult.Errors)
                {
                    _logger.LogError(error);
                    continue;
                }

                _errorGroupNumber++;
            }

            if (importResult.Readings.Count == 0)
            {
                _logger.LogError(LogMessages.NOREADINGIMPORTED);
                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                return importResult;
            }

            _logger.LogInformation(LogMessages.OBJECTCREATIONSUCCESS,
                                   importResult.Readings.Count,
                                   nameof(Reading));

            return importResult; 
        }

        private IList<ConsistentReading> CreateConsistentReadings(ImportResult importResult)
        {
            IList<ConsistentReading> consistentReadings =
                new List<ConsistentReading>();

            _logger.LogInformation(LogMessages.CREATEOBJECT, nameof(ConsistentReading));
            foreach (Reading reading in importResult.Readings)
            {
                var cr = _consistentReadingFactory.CreateConsistentReading(reading);
                if (!cr.Success)
                {
                    _logger.LogError(LogMessages.ERRORSFOUND, _errorGroupNumber);
                    foreach (var error in cr.Errors)
                    {
                        _logger.LogError(error);
                        continue;
                    }

                    _errorGroupNumber++;
                }

                else consistentReadings.Add(cr.Value);
            }

            if (consistentReadings.Count == 0)
            {
                _logger.LogError(LogMessages.NOOBJECTCREATED, nameof(ConsistentReading));
                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                return consistentReadings;
            }

            _logger.LogInformation(LogMessages.OBJECTCREATIONSUCCESS,
                                   consistentReadings.Count,
                                   nameof(ConsistentReading));

            return consistentReadings;
        }

        private IList<ProvinceData> CreateProvinceData(IList<ConsistentReading> consistentReadings)
        {
            _logger.LogInformation(LogMessages.CREATEOBJECT, nameof(ProvinceData));

            IList<ProvinceData> provinceDatas =
                _provinceDataListFactory.CreateProvinceDataList(consistentReadings);

            if (provinceDatas.Count == 0)
            {
                _logger.LogError(LogMessages.NOOBJECTCREATED, nameof(ProvinceData));
                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                return provinceDatas;
            }

            _logger.LogInformation(LogMessages.OBJECTCREATIONSUCCESS,
                                   provinceDatas.Count,
                                   nameof(ProvinceData));

            return provinceDatas;
        }

        private IList<AverageProvinceData> CreateAverageProvinceData(IList<ProvinceData> provinceDatas)
        {
            IList<AverageProvinceData> averageProvinceDatas =
                new List<AverageProvinceData>();

            _logger.LogInformation(LogMessages.CREATEOBJECT,
                                   nameof(AverageProvinceData));

            foreach (ProvinceData provinceData in provinceDatas)
            {
                var averageProvinceData = _averageProvinceDataFactory.CreateAverageProvinceData(provinceData);

                if (!averageProvinceData.Success)
                {
                    _logger.LogError(LogMessages.ERRORSFOUND, _errorGroupNumber);
                    foreach (var errors in averageProvinceData.Errors)
                    {
                        _logger.LogError(errors);
                        continue;
                    }

                    _errorGroupNumber++;
                }

                else averageProvinceDatas.Add(averageProvinceData.Value);
            }

            if (averageProvinceDatas.Count == 0)
            {
                _logger.LogError(LogMessages.NOOBJECTCREATED, nameof(AverageProvinceData));
                _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                return averageProvinceDatas;
            }

            _logger.LogInformation(LogMessages.OBJECTCREATIONSUCCESS,
                                   nameof(AverageProvinceData),
                                   averageProvinceDatas.Count);

            return averageProvinceDatas;
        }

        private void ExportAverageProvinceData(string noErrorsFilePath, IList<AverageProvinceData> averageProvinceDatas)
        {
            _logger.LogInformation(LogMessages.EXPORTAVERAGEPROVINCEDATA);
            using var fileStream = new FileStream(noErrorsFilePath,
                                                    FileMode.Create,
                                                    FileAccess.Write);

            Log.CloseAndFlush();

            _averageProvinceDataExportService.Export(averageProvinceDatas,
                                                        fileStream);
        }

        private string ExportDirectoryPathGeneratorAndLoggerConfiguration(string baseOutPath)
        {
            string specificReadingsAndErrorsDirectoryName =
                SystemTime.Now().ToString("yyyyMMdd_HHmmss");

            string fullFolderPath = Path.Combine(baseOutPath,
                            specificReadingsAndErrorsDirectoryName);

            Directory.CreateDirectory(fullFolderPath);

            string noErrorsFilePath = Path.Combine(fullFolderPath,
                                                   "AverageProvinceData.csv");

            string errorsFilePath = Path.Combine(fullFolderPath,
                                                 "Errors.log");

            LogPathHolder.ErrorLogPath = errorsFilePath;

            return noErrorsFilePath;
        }
    }
}