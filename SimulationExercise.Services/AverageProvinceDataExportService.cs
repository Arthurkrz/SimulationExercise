using FileHelpers;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Services
{
    public class AverageProvinceDataExportService : IAverageProvinceDataExportService
    {
        public void Export(IList<AverageProvinceData> averageProvinceData, Stream outputStream)
        {
            if (averageProvinceData == null)
                throw new ArgumentNullException(nameof(averageProvinceData),
                               "Null Average Province Data list.");

            if (averageProvinceData.Count == 0)
                throw new ArgumentException(nameof(averageProvinceData),
                                   "Empty Average Province Data list.");

            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream), 
                                              "Null Output Stream.");

            var engine = new FileHelperEngine<AverageProvinceDataExportDTO>();

            IList<AverageProvinceDataExportDTO> records = 
                new List<AverageProvinceDataExportDTO>();

            using (var sw = new StreamWriter(outputStream, leaveOpen: true))
            {
                records = averageProvinceData.Select(x => new AverageProvinceDataExportDTO
                                                    (x.Province, x.SensorTypeName,
                                                     x.AverageValue, x.Unit.ToString()
                                                                           .Replace("_", "/")
                                                                           .Replace("3", "³"),
                                                     x.AverageDaysOfMeasure)).ToList();

                engine.WriteStream(sw, records);
            }
        }
    }
}