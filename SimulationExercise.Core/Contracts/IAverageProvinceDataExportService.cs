using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts
{
    public interface IAverageProvinceDataExportService
    {
        void Export(IList<AverageProvinceData> averageProvinceData, Stream outputStream);
    }
}
