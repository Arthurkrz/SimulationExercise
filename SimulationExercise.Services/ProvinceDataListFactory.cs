using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;

namespace SimulationExercise.Services
{
    public class ProvinceDataListFactory : IProvinceDataListFactory
    {
        public IList<ProvinceData> CreateProvinceDataList
                                  (IList<ConsistentReading> 
                                        consistentReadings)
        {
            if (consistentReadings == null || 
                consistentReadings.Count == 0) 
                return new List<ProvinceData>();

            IList<ProvinceData> provinceDatas = new List<ProvinceData>();
            var groupedReadings = consistentReadings
                .GroupBy(cr => new { cr.Province, cr.SensorTypeName,
                                                  cr.Unit }).ToList();

            AverageProvinceDataFactory averageProvinceDataFactory =
                new AverageProvinceDataFactory();

            foreach (var group in groupedReadings) 
            {
                var readingsInGroup = group.ToList<ConsistentReading>();
                var provinceData = new ProvinceData(
                    group.Key.Province,
                    group.Key.SensorTypeName,
                    readingsInGroup);

                provinceDatas.Add(provinceData);
            }

            return provinceDatas;
        }
    }
}