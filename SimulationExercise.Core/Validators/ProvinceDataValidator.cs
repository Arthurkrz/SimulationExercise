using FluentValidation;
using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Validators
{
    public class ProvinceDataValidator : AbstractValidator<ProvinceData>
    {
        public ProvinceDataValidator()
        {
            this.RuleFor(x => x.Province)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty province name.");

            this.RuleFor(x => x.SensorTypeName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Null or empty sensor type name.");

            this.RuleFor(x => x.ConsistentReadings)
                .NotEmpty()
                .WithMessage("Consistent readings list cannot be empty.");

            this.RuleFor(x => x.ConsistentReadings.All(r => r.Province == x.ConsistentReadings.First().Province))
                .Equal(true)
                .WithMessage("Inconsistent provinces in readings.");

            this.RuleFor(x => x.ConsistentReadings.All(r => r.SensorTypeName == x.ConsistentReadings.First().SensorTypeName))
                .Equal(true)
                .WithMessage("Inconsistent sensor names in readings.");

            this.RuleFor(x => x.ConsistentReadings.All(r => r.Unit == x.ConsistentReadings.First().Unit))
                .Equal(true)
                .WithMessage("Inconsistent units in readings.");
        }
    }
}