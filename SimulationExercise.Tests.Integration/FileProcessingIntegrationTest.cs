using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
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
        public void Program_ShouldExportFiles_WithNoErrors(Stream inputStream, Stream expectedOutputStream)
        {
            // Arrange
            string inTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTest";
            string outTestDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTest";
            string inFilePath = Path.Combine(inTestDirectoryPath, "NoErrorFile.csv");

            if (!Directory.Exists(inTestDirectoryPath)) Directory.CreateDirectory(inTestDirectoryPath);
            if (!Directory.Exists(outTestDirectoryPath)) Directory.CreateDirectory(outTestDirectoryPath);

            var readingFile = "AverageProvinceData.csv";

            Stream outputStream = new MemoryStream();
            var writer = new StreamReader(outputStream);

            using FileStream inputFileStream =
            new FileStream(inFilePath,
                            FileMode.OpenOrCreate,
                            FileAccess.Write,
                            FileShare.Read);
            //{ using inputStreamWriter = new StreamWriter(inputStream) }

            // Act
            _sut.ProcessFile(inTestDirectoryPath, outTestDirectoryPath);

            var outFiles = Directory.GetFiles(outTestDirectoryPath);
            //var output = reader.ReadToEnd();

            // Assert
            Assert.Single(outFiles);
            Assert.Contains(readingFile, outFiles);
            //Assert.Equal(expectedOutputText, output);

            // Teardown
            Directory.Delete(outTestDirectoryPath, true);
            Directory.Delete(inTestDirectoryPath, true);
        }

        [Theory]
        [MemberData(nameof(InvalidStreamGenerator))]
        public void Program_ShouldReturnError_WhenNoAverageProvinceDataCreated(Stream streamInputWithErrors, Stream streamExpectedErrors)
        {
            // Arrange
            string outTestErrorDirectoryPath = @"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUTTestErrors";
            var errorFile = "Error.log";

            Stream outputStream = new MemoryStream();

            // Act
            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\INTestErrors",
                outTestErrorDirectoryPath);

            var outFiles = Directory.GetFiles(outTestErrorDirectoryPath);

            // Assert
            Assert.Single(outFiles);
            Assert.Contains(errorFile, outFiles);

            // Teardown
            Directory.Delete(outTestErrorDirectoryPath, true);
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