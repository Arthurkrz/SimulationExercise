using FluentAssertions;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Services;
using System;
using System.Text;

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
        public void Import_ShouldReturnException_WhenEmptyFile()
        {
            // Arrange
            MemoryStream stream = default;

            // Act & Assert
            var exception = Assert.Throws<FormatException>(() => _sut.Import(stream));
            Assert.Equal("Empty stream.", exception.Message);
        }

        [Fact]
        public void Import_ShouldReturnException_WhenNoCSVHeader()
        {
            // Arrange
            string documentText = @"12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
5712,Ozono,µg/m³,510,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
20488,Particelle sospese PM2.5,µg/m³,564,Erba v.Battisti,279,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)";

            var bytes = Encoding.UTF8.GetBytes(documentText);
            var stream = new MemoryStream(bytes);

            // Act & Assert
            var exception = Assert.Throws<FormatException>(() => _sut.Import(stream));
            Assert.Equal("Invalid header values.", exception.Message);
        }

        [Fact]
        public void Import_ShouldReturnImportResultNoErrors_WhenHeaderCorrect()
        {
            // Arrange
            string documentText = @"IdSensore, NomeTipoSensore, UnitaMisura, Idstazione, NomeStazione, Quota, Provincia, Comune, Storico, DataStart, DataStop, Utm_Nord, UTM_Est, lat, lng, Location
                12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
                5712,Ozono,µg/m³,510,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
                20488,Particelle sospese PM2.5,µg/m³,564,Erba v.Battisti,279,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)";

            var bytes = Encoding.UTF8.GetBytes(documentText);
            var stream = new MemoryStream(bytes);

            IList<Reading> expectedReadings = new List<Reading> 
            {   
                new Reading(12691, "Arsenico", "ng/m³", 560, "Varese v.Copelli", 383, "VA", "Varese", false, new DateTime(2008, 4, 1), null, 5073728, 486035, "45.8169745", "8.82024911"),
                new Reading(5712, "Ozono", "µg/m³", 510, "Inzago v.le Gramsci", 138, "MI", "Inzago", true, new DateTime(2001, 2, 24), new DateTime(2018, 1, 1), 5043030, 538012, "45.53976956", "9.48689669"),
                new Reading(20488, "Particelle sospese PM2.5", "µg/m³", 564, "Erba v.Battisti", 279, "CO", "Erba", false, new DateTime(2020, 10, 22), null, 5072803, 517232, "45.8085738", "9.2217792") 
            };

            // Act
            var result = _sut.Import(stream);

            // Assert
            Assert.Empty(result.Errors);
            result.Readings.Should().BeEquivalentTo(expectedReadings);
        }

        [Fact]
        public void Import_ShouldReturnImportResultErrors_WhenReadingsNotCorrect()
        {
            // Arrange
            string documentText = @"IdSensore, NomeTipoSensore, UnitaMisura, Idstazione, NomeStazione, Quota, Provincia, Comune, Storico, DataStart, DataStop, Utm_Nord, UTM_Est, lat, lng, Location
, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
ERROR, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,ERROR, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,ERROR, VA, Varese, N,01/04/2008,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,ERROR,,5073728,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,ERROR,486035,45.8169745,8.82024911, POINT (8.82024911 45.8169745)
12691, Arsenico, ng/m³,560, Varese v.Copelli,383, VA, Varese, N,01/04/2008,,5073728,ERROR,45.8169745,8.82024911, POINT (8.82024911 45.8169745)";

            IList<string> expectedErrors = new List<string>
            {
                "Line: 1 Column: 1. No value found for the value type field: '<IdSensore>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "Line: 2 Column: 24. No value found for the value type field: '<Idstazione>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "Line: 3 Column: 46. No value found for the value type field: '<Quota>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "Line: 4 Column: 65. No value found for the value type field: '<DataStart>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "Line: 5 Column: 77. No value found for the value type field: '<Utm_Nord>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "Line: 6 Column: 85. No value found for the value type field: '<UTM_Est>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "Error Converting 'ERROR' to type: 'Int64'. ",
                "Error Converting 'ERROR' to type: 'Int64'. ",
                "Error Converting 'ERROR' to type: 'Int32'. ",
                "Error Converting 'ERROR' to type: 'DateTime'.  There are less chars in the Input String than in the Format string: 'dd/MM/yyyy'",
                "Error Converting 'ERROR' to type: 'Int32'. ",
                "Error Converting 'ERROR' to type: 'Int32'. "

            };

            var bytes = Encoding.UTF8.GetBytes(documentText);
            var stream = new MemoryStream(bytes);

            // Act
            var result = _sut.Import(stream);

            // Assert
            Assert.Empty(result.Readings);
            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }
    }
}