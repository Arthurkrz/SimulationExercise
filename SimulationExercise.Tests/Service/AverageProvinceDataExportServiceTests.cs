using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Services;

namespace SimulationExercise.Tests.Service
{
    public class AverageProvinceDataExportServiceTests
    {
        private readonly IAverageProvinceDataExportService _sut;

        public AverageProvinceDataExportServiceTests()
        {
            _sut = new AverageProvinceDataExportService();
        }

        [Fact]
        public void Export_ShouldContainCorrectString()
        {
            // Arrange
            IList<AverageProvinceData> averageProvinceDatas = new List<AverageProvinceData>
            { 
                new AverageProvinceData("Province1", "Sensor2", 10, Unit.mg_m3, 25),
                new AverageProvinceData("Province1", "Sensor2", 15, Unit.mg_m3, 30),
                new AverageProvinceData("Province1", "Sensor2", 20, Unit.mg_m3, 35)
            };

            string expectedResult = "Province1,Sensor2,10,mg_m3,25\r\nProvince1,Sensor2,15,mg_m3,30\r\nProvince1,Sensor2,20,mg_m3,35\r\n";

            Stream outputStream = new MemoryStream();

            // Act
            _sut.Export(averageProvinceDatas, outputStream);

            // Assert
            outputStream.Position = 0;
            var reader = new StreamReader(outputStream);
            var output = reader.ReadToEnd();

            Assert.Equal(expectedResult, output);
        }

        [Fact]
        public void Export_ShouldReturnException_WhenNullStream()
        {
            // Arrange
            IList<AverageProvinceData> averageProvinceDatas = new List<AverageProvinceData>
            {
                new AverageProvinceData("Province1", "Sensor2", 10, Unit.mg_m3, 25),
                new AverageProvinceData("Province1", "Sensor2", 15, Unit.mg_m3, 30),
                new AverageProvinceData("Province1", "Sensor2", 20, Unit.mg_m3, 35)
            };

            Stream nullStream = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Export(averageProvinceDatas, nullStream));
        }

        [Fact]
        public void Export_ShouldReturnException_WhenProvinceDataListNull()
        {
            // Arrange
            IList<AverageProvinceData> nullList = null;
            Stream outputStream = new MemoryStream();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sut.Export(nullList, outputStream));
        }

        [Fact]
        public void Export_ShouldReturnException_WhenProvinceDataListEmpty()
        {
            // Arrange
            IList<AverageProvinceData> emptyList = new List<AverageProvinceData>();
            Stream outputStream = new MemoryStream();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.Export(emptyList, outputStream));
        }
    }
}