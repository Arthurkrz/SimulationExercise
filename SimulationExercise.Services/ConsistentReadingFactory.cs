using FluentValidation;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services
{
    public class ConsistentReadingFactory : IConsistentReadingFactory
    {
        private readonly IValidator<Reading> _validator;

        public ConsistentReadingFactory(IValidator<Reading> validator)
        {
            _validator = validator;
        }

        public Result<ConsistentReading> CreateConsistentReading
                                               (Reading reading)
        {
            string unitForConsistent = reading.Unit
                                              .Replace("/", "_")
                                              .Replace("³", "3");

            reading.Unit = unitForConsistent;
            var validationResult = _validator.Validate(reading);
            if (!validationResult.IsValid)
            {
                IList<string> errors = new List<string>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }

                return Result<ConsistentReading>.Ko(errors);
            }

            var unit = Enum.Parse<Unit>(reading.Unit);
            int daysOfMeasure = (reading.StopDate - reading.StartDate)
                                            .GetValueOrDefault().Days;

            ConsistentReading consistentReading =
                new ConsistentReading(reading.SensorId,
                reading.SensorTypeName, unit, reading.Value,
                reading.Province, reading.City, reading.IsHistoric,
                reading.UtmNord, reading.UtmEst, reading.Latitude,
                reading.Longitude, reading.Location)
                {
                    DaysOfMeasure = daysOfMeasure
                };

            return Result<ConsistentReading>.Ok(consistentReading);
        }
    }
}