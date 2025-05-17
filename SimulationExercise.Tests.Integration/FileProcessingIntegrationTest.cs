using System.Text;
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
    }
}//"Stop date is before start date."
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