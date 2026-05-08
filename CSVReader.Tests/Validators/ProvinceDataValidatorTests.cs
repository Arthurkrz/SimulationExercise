using FluentAssertions;
using CSVReader.Core.Entities;
using CSVReader.Core.Validators;
using CSVReader.Tests.ObjectGenerators;

namespace CSVReader.Tests.Validators
{
    public class ProvinceDataValidatorTests
    {
        private readonly ProvinceDataValidator _sut = new();

        [Theory]
        [MemberData(nameof(ProvinceDataData.GetInconsistentProvinceData), MemberType = typeof(ProvinceDataData))]
        public void Validator_ShouldReturnErrors_WhenValidationFails(ProvinceData provinceData, List<string> expectedErrors)
        {
            // Act
            var result = _sut.Validate(provinceData);

            // Assert
            Assert.False(result.IsValid);
            result.Errors.Select(e => e.ErrorMessage).Should().BeEquivalentTo(expectedErrors);
        }

        [Theory]
        [MemberData(nameof(ProvinceDataData.GetProvinceData), MemberType = typeof(ProvinceDataData))]
        public void Validator_ShouldNotReturnErrors_WhenValidProvinceData(List<ProvinceData> provinceDatas)
        {
            // Act & Assert
            foreach (var provinceData in provinceDatas)
                _sut.Validate(provinceData).Errors.Should().BeEmpty();
        }
    }
}
