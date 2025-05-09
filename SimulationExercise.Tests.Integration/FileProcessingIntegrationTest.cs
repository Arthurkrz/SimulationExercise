using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.IOC;
using System.Text;

namespace SimulationExercise.Tests.Integration
{
    public class FileProcessingIntegrationTest
    {
        private readonly IFileProcessingService _sut;
        private readonly ServiceProvider _serviceProvider;

        public FileProcessingIntegrationTest()
        {
            ServiceCollection services = new ServiceCollection();

            DependencyInjection.InjectServices(services);
            DependencyInjection.InjectValidators(services);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            _serviceProvider = services.BuildServiceProvider();
            _sut = _serviceProvider.GetRequiredService<IFileProcessingService>();
        }

        [Theory]
        [MemberData(nameof(ValidStreamGenerator))]
        public void Program_ShouldExportFiles_WithNoErrors(Stream inputStream, string expectedOutputText)
        {
            // Arrange
            string inTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTest";
            string outTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTest";
            string inFilePath = Path.Combine(inTestDirectoryPath, "NoErrorFile.csv");

            if (!Directory.Exists(inTestDirectoryPath)) Directory.CreateDirectory(inTestDirectoryPath);
            if (!Directory.Exists(outTestDirectoryPath)) Directory.CreateDirectory(outTestDirectoryPath);

            using (var fileStream = new FileStream(inFilePath, FileMode.Create, FileAccess.Write))
            {
                inputStream.Position = 0;
                inputStream.CopyTo(fileStream);
            }

            // Act
            _sut.ProcessFile(inTestDirectoryPath, outTestDirectoryPath);

            var exportDirectories = Directory.GetDirectories(outTestDirectoryPath);
            var resultOutFilePath = Path.Combine(exportDirectories[0], "AverageProvinceData.csv");

            var resultOutputText = File.ReadAllText(resultOutFilePath).Trim();

            // Assert
            Assert.Single(exportDirectories);
            Assert.True(File.Exists(resultOutFilePath);
            Assert.Equal(expectedOutputText.Trim(), resultOutputTexto);

            // Teardown
            Directory.Delete(outTestDirectoryPath, true);
            Directory.Delete(inTestDirectoryPath, true);
        }

        [Theory]
        [MemberData(nameof(InvalidStreamGenerator))]
        public void Program_ShouldReturnError_WhenNoAverageProvinceDataCreated(Stream streamInputWithErrors, string expectedErrorsText)
        {
            // Arrange
            string inTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTestErrors";
            string outTestErrorDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTestErrors";
            var inErrorFilePath = Path.Combine(inTestDirectoryPath, "ErrorFile.csv");

            if (!Directory.Exists(inTestDirectoryPath)) Directory.CreateDirectory(inTestDirectoryPath);
            if (!Directory.Exists(outTestErrorDirectoryPath)) Directory.CreateDirectory(outTestErrorDirectoryPath);

            using (var fileStream = new FileStream(inErrorFilePath, FileMode.Create, FileAccess.Write))
            {
                streamInputWithErrors.Position = 0;
                streamInputWithErrors.CopyTo(fileStream);
            }

            // Act
            _sut.ProcessFile(inTestDirectoryPath, outTestErrorDirectoryPath);

            var exportDirectories = Directory.GetDirectories(outTestErrorDirectoryPath);
            var resultOutErrorFilePath = Path.Combine(exportDirectories[0], "ErrorFile.log");

            var resultErrorOutputText = File.ReadAllText(resultOutErrorFilePath).Trim();

            // Assert
            Assert.Single(exportDirectories);
            Assert.True(File.Exists(resultOutErrorFilePath));
            Assert.Equal(expectedErrorsText.Trim(), resultErrorOutputText);

            // Teardown
            Directory.Delete(outTestErrorDirectoryPath, true);
            Directory.Delete(inTestDirectoryPath, true);
        }

        public static IEnumerable<object[]> ValidStreamGenerator()
        {
            string inputText = @"IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
5712,Ozono,µg/m³,510,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
20488,Ozono,µg/m³,564,Erba v. Battisti,279,MI,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)
10043,Arsenico,ng/m³,687,Ferno v.Di Dio,215,VA,Ferno,N,29/11/2006,,5051773,481053,45.61924753,8.75697656,POINT (8.75697656 45.61924753)
6342,Ossidi di Azoto,mg/m³,515,Pero SS Sempione,141,BS,Pero,S,10/12/1986,30/07/2018,5039595,507028,45.50985564,9.08997419,POINT (9.08997419 45.50985564)
6665,Ossidi di Azoto,mg/m³,565,Cantù v.Meucci,369,BS,Cantù,N,17/01/2005,,5064150,509783,45.73083728,9.12573936,POINT (9.12573936 45.73083728)
10270,Ozono,µg/m³,655,Darfo Boario Terme v. De Amicis,221,MI,Darfo Boario Terme,S,01/01/2007,12/07/2023,5080789,591371,45.87460256,10.17736553,POINT (10.17736553 45.87460256)
5507,Biossido di Azoto,µg/m³,504,Sesto San Giovanni v. Cesare da Sesto,139,CO,Sesto San Giovanni,N,18/12/1980,,5042386,518435,45.53476819,9.23610903,POINT (9.23610903 45.53476819)
10035,Benzene,µg/m³,672,Cornale v.Libertà,74,CO,Cornale,N,02/02/2006,,4987406,493238,45.04008561,8.91415717,POINT (8.91415717 45.04008561)
6831,Ossidi di Azoto,mg/m³,657,Lonato del Garda v.del Marchesino,180,BS,Lonato del Garda,N,01/01/1990,,5035536,615762,45.46375819,10.48078183,POINT (10.48078183 45.46375819)";

            string expectedOutputText = @"Province,SensorTypeName,AverageValue,Unit,AverageDaysOfMeasure
VA,Arsenico,299.00,ng_m3,6490
MI,Ozono,212.67,µg_m3,4608
BS,Ossidi di Azoto,230.00,mg_m3,10631
CO,Benzene,74.00,µg_m3,7056
CO,Biossido di Azoto,139.00,µg_m3,16244";

            var bytesInputText = Encoding.UTF8.GetBytes(inputText);
            var bytesExpectedOutputText = Encoding.UTF8.GetBytes(expectedOutputText);

            var streamInput = new MemoryStream(bytesInputText);
            var streamExpectedOutput = new MemoryStream(bytesExpectedOutputText);

            yield return new object[]
            {
                streamInput, streamExpectedOutput
            };
        }

        public static IEnumerable<object[]> InvalidStreamGenerator()
        {
            string inputWithErrorsText = @"";

            string expectedErrorsText = @"";

            var bytesInputWithErrorsText = Encoding.UTF8.GetBytes(inputWithErrorsText);
            var bytesExpectedErrorsText = Encoding.UTF8.GetBytes(expectedErrorsText);

            var streamInputWithErrors = new MemoryStream(bytesInputWithErrorsText);
            var streamExpectedErrors = new MemoryStream(bytesExpectedErrorsText);

            yield return new object[]
            {
                streamInputWithErrors, streamExpectedErrors
            };
        }
    }
}

// 1.Import (engine errors), 2.CR creation (validator errors), 3.APD creation, 4.Export

//OBJECTS 1
//@"IdSensore, NomeTipoSensore, UnitaMisura, Idstazione, NomeStazione, Quota, Provincia, Comune, Storico, DataStart, DataStop, Utm_Nord, UTM_Est, lat, lng, Location
//,Arsenico,ng/m³,,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//ERROR,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,ERROR,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,ERROR,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,ERROR,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,ERROR,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,ERROR,45.8169745,8.82024911,POINT (8.82024911 45.8169745)"

//ERRORS 1
//"Line: 1 Column: 0. The line 1 is empty. Maybe you need to use the attribute [IgnoreEmptyLines] in your record class.",
//"Line: 2 Column: 24. No value found for the value type field: '<Idstazione>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
//"Line: 3 Column: 46. No value found for the value type field: '<Quota>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
//"Line: 4 Column: 65. No value found for the value type field: '<DataStart>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
//"Line: 5 Column: 77. No value found for the value type field: '<Utm_Nord>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
//"Line: 6 Column: 85. No value found for the value type field: '<UTM_Est>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
//"Error Converting 'ERROR' to type: 'Int64'. ",
//"Error Converting 'ERROR' to type: 'Int64'. ",
//"Error Converting 'ERROR' to type: 'Int32'. ",
//"Error Converting 'ERROR' to type: 'DateTime'.  There are less chars in the Input String than in the Format string: 'dd/MM/yyyy'",
//"Error Converting 'ERROR' to type: 'Int32'. ",
//"Error Converting 'ERROR' to type: 'Int32'. "

//OBJECTS 2
//new Reading(0,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(-1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,null,"ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ERROR",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",0,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",-1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,null,1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",-1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","mg/m³",1,"Station Name",1,null,"City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","mg/m³",1,"Station Name",1,"Province",null,true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","µg/m³",1,"Station Name",1,"Province","",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,new DateTime(1967, 1, 1),DateTime.Now,1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now,DateTime.Now.AddYears(-1),1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,0,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,-1,1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,0,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,-1,"Latitude","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,null,"Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"","Longitude"),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude",null),
//new Reading(1,"Sensor Name","ng/m³",1,"Station Name",1,"Province","City",true,DateTime.Now.AddYears(-1),null,1,1,"Latitude",""),
//new Reading(0,null,"",0,null,-1,null,null,default,new DateTime(1967, 1, 1),new DateTime(1966, 1, 1),0, 0,null,null),
//new Reading(-1,"","ERROR",-1,"",-1,"","",default,new DateTime(1967, 1, 1),new DateTime(1966, 1, 1),-1, -1,"","")

//ERRORS 2
//"Sensor ID less or equal to 0."
//"Sensor ID less or equal to 0."
//"Null or empty sensor name."
//"Null or empty sensor name."
//"Unit not supported."
//"Station ID less or equal to 0."
//"Station ID less or equal to 0."
//"Null or empty station name."
//"Null or empty station name."
//"Negative value."
//"Null or empty province name."
//"Null or empty province name."
//"Null or empty city name."
//"Null or empty city name."
//"Start date is before the possible minimum."
//"Stop date is before start date."
//"UTMNord less or equal to 0."
//"UTMNord less or equal to 0."
//"UTMEst less or equal to 0."
//"UTMEst less or equal to 0."
//"Null or empty latitude."
//"Null or empty latitude."
//"Null or empty longitude."
//"Null or empty longitude."
//"Sensor ID less or equal to 0.","Null or empty sensor name.","Unit not supported.","Station ID less or equal to 0.","Null or empty station name.","Negative value.","Null or empty province name.","Null or empty city name.","Start date is before the possible minimum.","Stop date is before start date.","UTMNord less or equal to 0.","UTMEst less or equal to 0.","Null or empty latitude.","Null or empty longitude."
//"Sensor ID less or equal to 0.","Null or empty sensor name.","Unit not supported.","Station ID less or equal to 0.","Null or empty station name.","Negative value.","Null or empty province name.","Null or empty city name.","Start date is before the possible minimum.","Stop date is before start date.","UTMNord less or equal to 0.","UTMEst less or equal to 0.","Null or empty latitude.","Null or empty longitude."

//OBJECTS 3
//new ConsistentReading(13, "Sensor1", Unit.mg_m3, 100, "Province1", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 10 }
//new ConsistentReading(14, "Sensor2", Unit.ng_m3, 110, "Province2", "City", false, 1, 1, "Latitude", "Longitude") { DaysOfMeasure = 15 }

//ERRORS 3
//"Inconsistent provinces in readings."
//"Inconsistent units in readings."
//"Inconsistent sensor names in readings."

// EXCEPTION - PROVINCE DATA CONTAINS NO READINGS (STREAM + FILE)
// EXCEPTION - EXPORT ERRORS (STREAM + FILE)

//new Reading(0, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Sensor ID less or equal to 0." }

//new Reading(-1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Sensor ID less or equal to 0." }

//new Reading(1, null, "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty sensor name." }

//new Reading(1, "", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty sensor name." }

//new Reading(1, "Sensor Name", "ERROR", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Unit not supported." }

//new Reading(1, "Sensor Name", "ng/m³", 0,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Station ID less or equal to 0." }

//new Reading(1, "Sensor Name", "ng/m³", -1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Station ID less or equal to 0." }

//new Reading(1, "Sensor Name", "ng/m³", 1,null, 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty station name." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string { "Null or empty station name." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", -1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Negative value." }

//new Reading(1, "Sensor Name", "mg/m³", 1,"Station Name", 1, null,"City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty province name." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty province name." }

//new Reading(1, "Sensor Name", "mg/m³", 1,"Station Name", 1, "Province",null, true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty city name." }

//new Reading(1, "Station Name", "µg/m³", 1,"Station Name", 1, "Province","", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", "Longitude"),
//new List<string> { "Null or empty city name." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,new DateTime(1967, 1, 1),DateTime.Now, 1, 1,"Latitude", "Longitude"),
//new List<string> { "Start date is before the possible minimum." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now,DateTime.Now.AddYears(-1), 1, 1,"Latitude", "Longitude"),
//new List<string> { "Stop date is before start date." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 0, 1, "Latitude", "Longitude"),
//new List<string> { "UTMNord less or equal to 0." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, -1, 1, "Latitude", "Longitude"),
//new List<string> { "UTMNord less or equal to 0." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 0, "Latitude", "Longitude"),
//new List<string> { "UTMEst less or equal to 0." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, -1, "Latitude", "Longitude"),
//new List<string> { "UTMEst less or equal to 0." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, null, "Longitude"),
//new List<string> { "Null or empty latitude." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "", "Longitude"),
//new List<string> { "Null or empty latitude." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", null),
//new List<string> { "Null or empty longitude." }

//new Reading(1, "Sensor Name", "ng/m³", 1,"Station Name", 1, "Province","City", true,DateTime.Now.AddYears(-1),null, 1, 1, "Latitude", ""),
//new List<string> { "Null or empty longitude." }

//new Reading(0,null,"",0,null,-1,null,null,default,new DateTime(1967, 1, 1),new DateTime(1966, 1, 1),0, 0,null,null)
//new List<string> { "Sensor ID less or equal to 0.","Null or empty sensor name.","Unit not supported.","Station ID less or equal to 0.","Null or empty station name.","Negative value.","Null or empty province name.","Null or empty city name.","Start date is before the possible minimum.","Stop date is before start date.","UTMNord less or equal to 0.","UTMEst less or equal to 0.","Null or empty latitude.","Null or empty longitude." }

//new Reading(-1,"","ERROR", -1,"", -1,"","", default,new DateTime(1967, 1, 1),new DateTime(1966, 1, 1),-1, -1,"","")
//new List<string> { "Sensor ID less or equal to 0.","Null or empty sensor name.","Unit not supported.","Station ID less or equal to 0.","Null or empty station name.","Negative value.","Null or empty province name.","Null or empty city name.","Start date is before the possible minimum.","Stop date is before start date.","UTMNord less or equal to 0.","UTMEst less or equal to 0.","Null or empty latitude.","Null or empty longitude." }