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

        public App(IReadingImportService readingImportService,
                   IConsistentReadingFactory consistentReadingFactory, 
                   IProvinceDataListFactory provinceDataListFactory,
                   IAverageProvinceDataFactory averageProvinceDataFactory,
                   IAverageProvinceDataExportService averageProvinceDataExportService)
        {
            _readingImportService = readingImportService;
            _consistentReadingFactory = consistentReadingFactory;
            _provinceDataListFactory = provinceDataListFactory;
            _averageProvinceDataFactory = averageProvinceDataFactory;
            _averageProvinceDataExportService = averageProvinceDataExportService;
        }

        public ImportResult ImportReadings(string inDirectoryPath)
        {
            if (string.IsNullOrEmpty(inDirectoryPath)) 
                throw new ArgumentNullException(nameof(inDirectoryPath), 
                                          "Empty 'IN' directory path.");

            if (Directory.Exists(inDirectoryPath))
            {
                var files = Directory.GetFiles(inDirectoryPath, "*.csv");
                if (files.Length == 0) throw new FileNotFoundException
                        ("No CSV files found in the 'IN' directory.");
             
                using var stream = new FileStream(files[0], FileMode.Open, FileAccess.Read);

                return _readingImportService.Import(stream);
            }

            else throw new DirectoryNotFoundException
                ("No 'IN' directory found in Console project folder.");
        }

        public IList<Result<ConsistentReading>> CreateConsistentReadings(IList<Reading> readings)
        {
            IList<Result<ConsistentReading>> consistentReadings = new List<Result<ConsistentReading>>();

            foreach (Reading reading in readings)
            {
                _consistentReadingFactory.CreateConsistentReading(reading);
                readings.Add(reading);
            }

            return consistentReadings;
        }

        public IList<ProvinceData> CreateProvinceDataList(IList<ConsistentReading> consistentReadings)
        {
            IList<ProvinceData> provinceDatas = new List<ProvinceData>();

            foreach (ProvinceData provinceData in provinceDatas)
            {
                _provinceDataListFactory.CreateProvinceDataList(consistentReadings);
                provinceDatas.Add(provinceData);
            }

            return provinceDatas;
        }

        public IList<Result<AverageProvinceData>> CreateAverageProvinceData(IList<ProvinceData> provinceDatas)
        {
            IList<Result<AverageProvinceData>> averageProvinceDatas =
                new List<Result<AverageProvinceData>>();

            foreach (ProvinceData provinceData in provinceDatas)
            {
                _averageProvinceDataFactory.CreateAverageProvinceData(provinceData);
                provinceDatas.Add(provinceData);
            }

            return averageProvinceDatas;
        }

        public void ExportAverageProvinceData(string outDirectoryPath, DateTime exportDate, IList<AverageProvinceData> averageProvinceDatas, Stream outputStream)
        {
            if (string.IsNullOrEmpty(outDirectoryPath)) 
                throw new ArgumentNullException(nameof(outDirectoryPath), 
                                           "Empty 'OUT' directory path");

            if (Directory.Exists(outDirectoryPath))
            {
                string specificReadingsAndErrorsDirectoryName = exportDate.ToString("yyyyMMdd_HHmmss");
                string fullPath = Path.Combine(outDirectoryPath, specificReadingsAndErrorsDirectoryName);
                Directory.CreateDirectory(fullPath);

                _averageProvinceDataExportService.Export(averageProvinceDatas, outputStream);
            }

            else throw new DirectoryNotFoundException
                    ("No 'OUT' directory found in Console project folder.");
        }

        //public void ExportErrors(IDictionary<ErrorType, IList<string>> errorsWithTypes)
        //{
        //    string[] errorFileName = errorsWithTypes.Keys.Select(key => key.ToString()).ToArray();
        //    errorsWithTypes.Values.
        //}
    }
}

// AverageProvinceDataFactory e ConsistentReadingFactory retornam Result
// ProvinceDataListFactory retorna uma lista possivelmente vazia
// ReadingImportService retorna um ImportResult
// AverageProvinceDataExportService pode retornar exceção