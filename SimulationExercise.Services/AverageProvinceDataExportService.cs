using FileHelpers;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Services
{
    public class AverageProvinceDataExportService : IAverageProvinceDataExportService
    {
        public void Export(IList<AverageProvinceData> averageProvinceData, Stream outputStream)
        {
            if (averageProvinceData == null)
            {
                throw new ArgumentNullException(nameof(averageProvinceData),
                               "Null Average Province Data list.");
            }

            if (averageProvinceData.Count == 0)
            {
                throw new ArgumentException(nameof(averageProvinceData),
                                   "Empty Average Province Data list.");
            }

            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream), 
                                              "Null Output Stream.");
            }

            var engine = new FileHelperEngine<AverageProvinceDataExportDTO>();
            IList<AverageProvinceDataExportDTO> records = new List<AverageProvinceDataExportDTO>();

            using (var sw = new StreamWriter(outputStream, leaveOpen: true))
            {
                foreach (var aPD in averageProvinceData)
                {
                    string DTOUnit = aPD.Unit.ToString()
                                             .Replace("_", "/")
                                             .Replace("3", "³");

                    AverageProvinceDataExportDTO APDDTO = new AverageProvinceDataExportDTO
                                                              (aPD.Province, aPD.SensorTypeName,
                                                               aPD.AverageValue, DTOUnit,
                                                               aPD.AverageDaysOfMeasure);
                    records.Add(APDDTO);
                }

                engine.WriteStream(sw, records.ToArray());
            }
        }
    }
}