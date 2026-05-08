using FluentValidation;
using CSVReader.Core.Contracts.Factories;
using CSVReader.Core.Entities;
using CSVReader.Core.Validators;
using CSVReader.Services.Factory;
using CSVReader.Tests.ObjectGeneration;

namespace CSVReader.Tests.Factories
{
    public class ConsistentReadingFactoryTests
    {
        private readonly IValidator<Reading> _readingValidator;
        private readonly IConsistentReadingFactory _sut;

        public ConsistentReadingFactoryTests()
        {
            _readingValidator = new ReadingValidator();
            _sut = new ConsistentReadingFactory(_readingValidator);
        }

        [Theory]
        [MemberData(nameof(ReadingData.GetValidReadings), MemberType = typeof(ReadingData))]
        public void CreateConsistentReading_ShouldCreateObject_WhenCorrectReadings(Reading reading)
        {
            // Act
            var result = _sut.CreateConsistentReading(reading);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.True(result.Success);
            Assert.Null(result.Errors);
        }

        [Theory]
        [MemberData(nameof(ReadingData.GetInvalidReadings), MemberType = typeof(ReadingData))]
        public void CreateConsistentReading_ShouldReturnErrors_WhenWrongReadings(Reading reading, List<string> errors)
        {
            // Act
            var result = _sut.CreateConsistentReading(reading);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(errors, result.Errors);
        }
    }
}