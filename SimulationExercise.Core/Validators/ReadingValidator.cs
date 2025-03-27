using FluentValidation;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Validators
{
    public class ReadingValidator : AbstractValidator<Reading>
    {
        public ReadingValidator()
        {
            this.RuleFor(x => x.SensorId).GreaterThan(0)
                                         .WithMessage
                                         ("Null or negative" +
                                          " sensor ID.");

            this.RuleFor(x => x.SensorTypeName).NotEmpty()
                                               .WithMessage
                                               ("Null or empty" +
                                               " sensor name.")
                                               .NotNull()
                                               .WithMessage
                                               ("Null or empty" +
                                               " sensor name.");

            this.RuleFor(x => x.Unit).IsEnumName(typeof(Unit))
                                     .WithMessage("Unit not " +
                                                  "supported.");

            this.RuleFor(x => x.StationId).GreaterThan(0)
                                          .WithMessage("Null or " +
                                            "negative station ID.");

            this.RuleFor(x => x.StationName).NotEmpty()
                                            .WithMessage("Null or " +
                                               "empty station name.")
                                            .NotNull()
                                            .WithMessage("Null or " +
                                               "empty station name.");

            this.RuleFor(x => x.Value).GreaterThan(-1)
                                      .WithMessage("Negative value.");

            this.RuleFor(x => x.Province).NotEmpty()
                                         .WithMessage("Null province" +
                                                              " name.")
                                         .NotNull()
                                         .WithMessage("Null province" +
                                                              " name.");

            this.RuleFor(x => x.City).NotEmpty()
                                     .WithMessage("Null or empty " +
                                                       "city name.")
                                     .NotNull()
                                     .WithMessage("Null or empty " +
                                                       "city name.");

            this.RuleFor(x => x.StartDate.Date).GreaterThan
                                                (new DateTime
                                                 (1968, 1, 1))
                                               .WithMessage("Start date" +
                                                       " is before the " +
                                                       "possible minimum.");

            this.RuleFor(x => x.StopDate).GreaterThanOrEqualTo
                                          (x => x.StartDate.Date)
                                          .When(x => x.StopDate.HasValue)
                                          .WithMessage("Stop date is " +
                                                    "before start date");

            this.RuleFor(x => x.UtmNord).GreaterThan(0)
                                        .WithMessage("Null or negative" +
                                                             " UTMNord.");

            this.RuleFor(x => x.UtmEst).GreaterThan(0)
                                       .WithMessage("Null or negative" +
                                                             " UTMEst.");

            this.RuleFor(x => x.Latitude).NotEmpty()
                                         .WithMessage("Null or empty" +
                                                          " latitude.")
                                         .NotNull()
                                         .WithMessage("Null or empty" +
                                                          " latitude.");

            this.RuleFor(x => x.Longitude).NotEmpty()
                                          .WithMessage("Null or empty" +
                                                          " longitude.")
                                          .NotNull()
                                          .WithMessage("Null or empty" +
                                                          " longitude.");
        }
    }

}
