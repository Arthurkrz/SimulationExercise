using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Services.Factory
{
    public class ProvinceDataListFactory : IProvinceDataListFactory
    {
        public IList<ProvinceData> CreateProvinceDataList(IList<ConsistentReading> consistentReadings)
        {
            if (consistentReadings == null || 
                consistentReadings.Count == 0) 
                return new List<ProvinceData>();

            var groupedReadings = consistentReadings
                .GroupBy(cr => new { cr.Province, cr.SensorTypeName,
                                                  cr.Unit }).ToList();

            var provinceDatas = groupedReadings.Select(cr => new ProvinceData
                                                      (cr.Key.Province, 
                                                       cr.Key.SensorTypeName, 
                                                       cr.ToList())).ToList();

            return provinceDatas;
        }
    }
}