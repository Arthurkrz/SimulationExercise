using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Services.Factory
{
    public class AverageProvinceDataFactory : IAverageProvinceDataFactory
    {
        public Result<AverageProvinceData> CreateAverageProvinceData(ProvinceData provinceData)
        {
            List<string> errors = IsEachReadingEqual(provinceData);
            if (errors.Count == 0)
            {
                double averageValue = Math.Round(provinceData
                    .ConsistentReadings.Average(r => r.Value), 2);
                int averageDaysOfMeasure = (int)provinceData
                    .ConsistentReadings.Average(r => r.DaysOfMeasure);
                var unit = provinceData.ConsistentReadings[0].Unit;

                var averageProvinceData = new AverageProvinceData(
                    provinceData.Province, provinceData.SensorTypeName,
                    averageValue, unit, averageDaysOfMeasure);

                return Result<AverageProvinceData>.Ok(averageProvinceData);
            }

            return Result<AverageProvinceData>.Ko(errors);
        }

        private List<string> IsEachReadingEqual(ProvinceData provinceData)
        {
            List<string> errors = new List<string>();

            if (provinceData.ConsistentReadings.Count == 1)
                return errors;

            if (provinceData.ConsistentReadings.Count == 0)
            {
                errors.Add("ProvinceData contains no readings.");
                return errors;
            }

            if (!provinceData.ConsistentReadings.All
               (r => r.Province == provinceData.ConsistentReadings
                                               .First().Province)) 
                errors.Add("Inconsistent provinces in readings.");

            if (!provinceData.ConsistentReadings.All
               (r => r.Unit == provinceData.ConsistentReadings
                                           .First().Unit))
                errors.Add("Inconsistent units in readings.");

            if (!provinceData.ConsistentReadings.All
               (r => r.SensorTypeName == provinceData.ConsistentReadings
                                                     .First()
                                                     .SensorTypeName))
                errors.Add("Inconsistent sensor names in readings.");

            return errors;
        }
    }
}