using FluentValidation;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services.Factory
{
    public class ConsistentReadingFactory : IConsistentReadingFactory
    {
        private readonly IValidator<Reading> _validator;

        public ConsistentReadingFactory(IValidator<Reading> validator)
        {
            _validator = validator;
        }

        public Result<ConsistentReading> CreateConsistentReading (Reading reading)
        {
            reading.Unit = reading.Unit switch
            {
                "ng/m³" => Unit.ng_m3.ToString(),
                "mg/m³" => Unit.mg_m3.ToString(),
                "µg/m³" => Unit.µg_m3.ToString(),
                _ => ""
            };

            var validationResult = _validator.Validate(reading);
            if (!validationResult.IsValid)
            {
                IList<string> errors = new List<string>();
                foreach (var error in validationResult.Errors)
                    errors.Add(error.ErrorMessage);

                return Result<ConsistentReading>.Ko(errors);
            }

            Enum.TryParse(reading.Unit, out Unit unit);
            int daysOfMeasure = (reading.StopDate - reading.StartDate)
                                            .GetValueOrDefault().Days;

            ConsistentReading consistentReading =
                new ConsistentReading(reading.SensorId,
                reading.SensorTypeName, unit, reading.Value,
                reading.Province, reading.City, reading.IsHistoric,
                reading.UtmNord, reading.UtmEst, reading.Latitude,
                reading.Longitude)
                { DaysOfMeasure = daysOfMeasure };

            return Result<ConsistentReading>.Ok(consistentReading);
        }
    }
}