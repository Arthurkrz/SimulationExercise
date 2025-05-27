using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IAverageProvinceDataExportService
    {
        void Export(IList<AverageProvinceData> averageProvinceData, Stream outputStream);
    }
}
