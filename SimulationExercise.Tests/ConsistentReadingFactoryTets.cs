using FluentValidation;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Validators;
using SimulationExercise.Services.Factory;
using SimulationExercise.Tests.ObjectGeneration;

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

        [Theory]
        [MemberData(nameof(ReadingData.GetValidReadings), MemberType = typeof(ReadingData))]
        public void CreateConsistentReading_ShouldCreateObject_WhenCorrectReadings
                                                                 (Reading reading)
        {
            // Act & Assert
            var result = _sut.CreateConsistentReading(reading);

            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.True(result.Success);
            Assert.Null(result.Errors);
        }

        [Theory]
        [MemberData(nameof(ReadingData.GetInvalidReadings), MemberType = typeof(ReadingData))]
        public void CreateConsistentReading_ShouldReturnErrors_WhenWrongReadings
                                          (Reading reading, List<string> errors)
        {
            // Act & Assert
            var result = _sut.CreateConsistentReading(reading);

            Assert.NotNull(result);
            Assert.Equal(errors, result.Errors);
        }
    }
}