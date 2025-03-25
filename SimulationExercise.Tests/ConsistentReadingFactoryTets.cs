using FluentValidation;
using SimulationExercise.Core;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Validators;
using SimulationExercise.Services;

namespace SimulationExercise.Tests
{
    public class ConsistentReadingFactoryTets
    {
        private readonly IValidator<Reading> _readingValidator;
        private readonly IConsistentReadingFactory _sut;

        public ConsistentReadingFactoryTets()
        {
            _readingValidator = new ReadingValidator();
            _sut = new ConsistentReadingFactory(_readingValidator);
        }

        [Fact]
        public void CreateConsistentReading_ShouldCreateObject_WhenCorrectReadings()
        {
            // Arrange
            Reading reading = new Reading(1,"Sensor Name","ng/m³",1,
                                          "Station Name",1,"Province",
                                          "City",true,
                                          DateTime.Now.AddYears(-1),
                                          null,1,1, "Latitude","Longitude",
                                          "Location");

            // Act & Assert
            var result = _sut.CreateConsistentReading(reading);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.True(result.Success);
            Assert.Null(result.Errors);
        }

        [Fact]
        public void CreateConsistentReading_ShouldReturnErrors_WhenWrongReadings()
        {
            // Arrange
            DateTime wrongStartDate = new DateTime(1967,1,1);
            DateTime wrongStopDate = new DateTime(1966, 1, 1);

            Reading reading = new Reading(0,"","",0,"",-1,"","",
                              default,wrongStartDate,wrongStopDate,
                                                      0,0,"","","");

            List<string> errors = new List<string>
            {
                "Null or negative sensor ID.",
                "Null or empty sensor name.",
                "Unit not supported.",
                "Null or negative station ID.",
                "Null or empty station name.",
                "Negative value.", 
                "Null province name.",
                "Null or empty city name.",
                "Start date is before the possible minimum.",
                "Stop date is before start date", 
                "Null or negative UTMNord.", 
                "Null or negative UTMEst.",
                "Null or empty latidude.",
                "Null or empty longitude."
            };

            // Act & Assert
            var result = _sut.CreateConsistentReading(reading);

            Assert.NotNull(result);
            Assert.Equal(errors, result.Errors);
        }
    }
}