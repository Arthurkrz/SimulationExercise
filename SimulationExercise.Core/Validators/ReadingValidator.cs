using FluentValidation;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Validators
{
    public class ReadingValidator : AbstractValidator<Reading>
    {
        public ReadingValidator()
        {
            this.RuleFor(x => x.SensorId)
                .GreaterThan(0)
                .WithMessage("Sensor ID less or equal to 0.");

            this.RuleFor(x => x.SensorTypeName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty sensor name.");

            this.RuleFor(x => x.Unit)
                .IsEnumName(typeof(Unit))
                .WithMessage("Unit not supported.");

            this.RuleFor(x => x.StationId)
                .GreaterThan(0)
                .WithMessage("Station ID less or equal to 0.");

            this.RuleFor(x => x.StationName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty station name.");

            this.RuleFor(x => x.Value)
                .GreaterThan(-1)
                .WithMessage("Negative value.");

            this.RuleFor(x => x.Province)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty province name.");

            this.RuleFor(x => x.City)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty city name.");

            this.RuleFor(x => x.StartDate.Date)
                .GreaterThan(new DateTime(1968, 1, 1))
                .WithMessage("Start date is before the possible minimum.");

            this.RuleFor(x => x.StopDate)
                .GreaterThanOrEqualTo(x => x.StartDate.Date)
                .When(x => x.StopDate.HasValue)
                .WithMessage("Stop date is before start date.");

            this.RuleFor(x => x.UtmNord)
                .GreaterThan(0)
                .WithMessage("UTMNord less or equal to 0.");

            this.RuleFor(x => x.UtmEst)
                .GreaterThan(0)
                .WithMessage("UTMEst less or equal to 0.");

            this.RuleFor(x => x.Latitude)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty latitude.");

            this.RuleFor(x => x.Longitude)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty longitude.");
        }
    }
}
