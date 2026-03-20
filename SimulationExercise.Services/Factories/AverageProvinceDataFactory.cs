using FluentValidation;
using SimulationExercise.Core.Contracts.Factories;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Services.Factory
{
    public class AverageProvinceDataFactory : IAverageProvinceDataFactory
    {
        private readonly IValidator<ProvinceData> _validator;

        public AverageProvinceDataFactory(IValidator<ProvinceData> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public IList<Result<AverageProvinceData>> CreateAverageProvinceData(IList<ConsistentReading> consistentReadings)
        {
            if (consistentReadings == null || consistentReadings.Count == 0)
                return new List<Result<AverageProvinceData>>
                    { Result<AverageProvinceData>.Ko(
                      new List<string> { "Null or empty consistent reading list" }) };

            var apdCreationResult = new List<Result<AverageProvinceData>>();
            List<ProvinceData> provinceDatas = new();

            var groupedReadings = consistentReadings
                .GroupBy(cr => new { cr.Province,
                                     cr.SensorTypeName,
                                     cr.Unit }).ToList();

            provinceDatas = groupedReadings.Select(cr => new ProvinceData
                                                    (cr.Key.Province,
                                                    cr.Key.SensorTypeName,
                                                    cr.ToList())).ToList();

            foreach (var provinceData in provinceDatas)
            {
                var validationResult = _validator.Validate(provinceData);

                if (!validationResult.IsValid)
                    return new List<Result<AverageProvinceData>>
                { Result<AverageProvinceData>.Ko(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList()) };

                double averageValue = Math.Round(provinceData
                    .ConsistentReadings.Average(r => r.Value), 2);

                int averageDaysOfMeasure = (int)provinceData
                    .ConsistentReadings.Average(r => r.DaysOfMeasure);

                var unit = provinceData.ConsistentReadings[0].Unit;

                var averageProvinceData = new AverageProvinceData
                (
                    provinceData.Province,
                    provinceData.SensorTypeName,
                    averageValue, unit,
                    averageDaysOfMeasure
                );

                apdCreationResult.Add(Result<AverageProvinceData>.Ok(averageProvinceData));
            }

            return apdCreationResult;
        }
    }
}