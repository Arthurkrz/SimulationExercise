using SimulationExercise.Core.Contracts;
using SimulationExercise.Services;

namespace SimulationExercise.Tests
{
    public class ReadingImportServiceTests
    {
        private readonly IReadingImportService _sut;

        public ReadingImportServiceTests()
        {
            _sut = new ReadingImportService();
        }

        [Fact]
        public void Import_ShouldReturnException_WhenCSVHeaderIncorrect()
        {
            // Arrange
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("Incorrect,Header,Values");
                writer.Flush();
                stream.Position = 0;
            }

            // Act & Assert
            var exception = Assert.Throws<FormatException>(() => _sut.Import(stream));
            Assert.Equal("Invalid header values.", exception.Message);
        }

        [Fact]
        public void Import_ShouldReturnImportResultNoErrors_WhenHeaderCorrect()
        {
            // Arrange
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("IdSensore, NomeTipoSensore, UnitaMisura, Idstazione, NomeStazione, Quota, Provincia, Comune, Storico, DataStart, DataStop, Utm_Nord, UTM_Est, lat, lng, Location");
                writer.WriteLine("12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)");
                writer.WriteLine("5712, Ozono, µg/m³,510, Inzago v.le Gramsci,138, MI, Inzago, S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669, POINT (9.48689669 45.53976956)");
                writer.WriteLine("20488, Particelle sospese PM2.5, µg/m³,564, Erba v. Battisti,279, CO, Erba, N,22/10/2020,,5072803,517232,45.8085738,9.2217792, POINT (9.2217792 45.8085738)");
                writer.Flush();
                stream.Position = 0;
            }

            // Act
            var result = _sut.Import(stream);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Readings);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Import_ShouldReturnImportResultErrors_WhenReadingsCorrect()
        {
            // Arrange
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("IdSensore, NomeTipoSensore, UnitaMisura, Idstazione, NomeStazione, Quota, Provincia, Comune, Storico, DataStart, DataStop, Utm_Nord, UTM_Est, lat, lng, Location");
                writer.WriteLine("ERROR, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)");
                writer.WriteLine("ERROR, Ozono, µg/m³,510, Inzago v.le Gramsci,138, MI, Inzago, S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669, POINT (9.48689669 45.53976956)");
                writer.WriteLine("ERROR, Particelle sospese PM2.5, 00000,564, Erba v. Battisti,279, CO, Erba, N,22/10/2020,,5072803,517232,45.8085738,9.2217792, POINT (9.2217792 45.8085738)");
                writer.Flush();
            }
            stream.Position = 0;

            // Act
            var result = _sut.Import(stream);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Errors);
            Assert.Empty(result.Readings);
        }
    }
}