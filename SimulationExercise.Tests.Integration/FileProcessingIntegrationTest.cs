//using FluentValidation;
//using FluentAssertions;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using SimulationExercise.Console;
//using SimulationExercise.Core.Contracts;
//using SimulationExercise.Core.Entities;
//using SimulationExercise.Core.Validators;
//using SimulationExercise.IOC;
//using SimulationExercise.Services;
//using SimulationExercise.Services.Factory;
//using SimulationExercise.Core.Enum;
//using System.Text;

//namespace SimulationExercise.Tests.Integration
//{
//    public class FileProcessingIntegrationTest
//    {
//        private readonly IFileProcessingService _sut;
//        private readonly ServiceProvider _serviceProvider;

//        public FileProcessingIntegrationTest()
//        {
//            var serviceCollection = new ServiceCollection();

//            DependencyInjection.InjectServices(serviceCollection);
//            DependencyInjection.InjectValidators(serviceCollection);

//            serviceCollection.AddLogging(config =>
//            {
//                config.AddConsole();
//                config.SetMinimumLevel(LogLevel.Information);
//            });

//            _serviceProvider = serviceCollection.BuildServiceProvider();

//            var readingImportService = _serviceProvider.GetRequiredService<IReadingImportService>();
//            var consistentReadingFactory = _serviceProvider.GetRequiredService<IConsistentReadingFactory>();
//            var provinceDataListFactory = _serviceProvider.GetRequiredService<IProvinceDataListFactory>();
//            var averageProvinceDataFactory = _serviceProvider.GetRequiredService<IAverageProvinceDataFactory>();
//            var averageProvinceDataExportService = _serviceProvider.GetRequiredService<IAverageProvinceDataExportService>();
//            var logger = _serviceProvider.GetRequiredService<ILogger<FileProcessingService>>();
//            _sut = _serviceProvider.GetRequiredService<IFileProcessingService>();
//        }

//        [Theory]
//        [MemberData(nameof(GetValidTestData))]
//        public void App_ShouldExportFiles_WithNoErrors(Stream csvReadingsStream, Stream expectedAverageProvinceDataStream)
//        {
//            // Act
//            _sut.ProcessFile(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\IN", 
//                                                   "TestFile.csv");

//            IList<Reading> filteredResultStep1 = resultStep1.Readings.ToList();

//            var resultStep2 = _sut.CreateConsistentReadings(resultStep1.Readings.ToList());
//            IList<ConsistentReading> filteredResultStep2 = resultStep2
//                                                           .Where(x => x.Success)
//                                                           .Select(x => x.Value)
//                                                           .ToList();

//            var resultStep3 = _sut.CreateProvinceDataList(filteredResultStep2);

//            var resultStep4 = _sut.CreateAverageProvinceData(resultStep3);
//            IList<AverageProvinceData> filteredResultStep4 = resultStep4
//                                                             .Where(x => x.Success)
//                                                             .Select(x => x.Value)
//                                                             .ToList();

//            DateTime folderName = DateTime.Now;
//            _sut.ExportAverageProvinceData(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUT",
//                                           folderName, filteredResultStep4, "TestFileNoErrors.csv");
//            var outFiles = Directory.GetFiles(Path.Combine(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUT", 
//                                                          folderName.ToString()));

//            Stream outputStream = new MemoryStream();
//            var reader = new StreamReader(Path.Combine(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise.Tests.Integration\OUT",
//                                                          folderName.ToString(), "TesFileNoErrors.csv"));
//            var output = reader.ReadToEnd();

//            // Assert
//            resultStep1.Readings.Should().BeEquivalentTo(expectedImportedReadings);
//            Assert.True(resultStep1.Errors.Count == 0);

//            filteredResultStep2.Should().BeEquivalentTo(expectedCreatedConsistentReadings);
//            Assert.DoesNotContain(resultStep2, x => x.Success == false);

//            resultStep3.Should().BeEquivalentTo(expectedCreatedProvinceData);

//            filteredResultStep4.Should().BeEquivalentTo(expectedCreatedAverageProvinceData);
//            Assert.DoesNotContain(resultStep4, x => x.Success == false);

//            Assert.True(outFiles.Length == 1);
//            Assert.Equal(expectedOutFileStream, output);
//        }

//        public static IEnumerable<object[]> GetValidTestData()
//        {
//            string noErrorFileText = @"12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
//5712,Ozono,µg/m³,510,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)
//20488,Particelle sospese PM2.5,µg/m³,564,Erba v.Battisti,279,CO,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)
//10043,PM10 (SM2005),µg/m³,687,Ferno v.Di Dio,215,VA,Ferno,N,29/11/2006,,5051773,481053,45.61924753,8.75697656,POINT (8.75697656 45.61924753)
//6342,Ossidi di Azoto,µg/m³,515,Pero SS Sempione,141,MI,Pero,N,10/12/1986,30/07/2018,5039595,507028,45.50985564,9.08997419,POINT (9.08997419 45.50985564)
//6665,Ozono,µg/m³,565,Cantù v.Meucci,369,CO,CantÃ¹,Âµg/mÂ³,17/01/2005,,5064150,509783,45.73083728,9.12573936,POINT (9.12573936 45.73083728)
//10270,Ozono,µg/m³,655,Darfo Boario Terme v. De Amicis,221,BS,Darfo Boario Terme,N,01/01/2007,12/07/2023,5080789,591371,45.87460256,10.17736553,POINT (10.17736553 45.87460256)
//5507,Biossido di Azoto,µg/m³,504,Sesto San Giovanni v. Cesare da Sesto,139,MI,Sesto San Giovanni,N,18/12/1980,,5042386,518435,45.53476819,9.23610903,POINT (9.23610903 45.53476819)
//10035,Benzene,µg/m³,672,Cornale v.Libertà ,74,PV,Cornale,N,02/02/2006,,4987406,493238,45.04008561,8.91415717,POINT (8.91415717 45.04008561)
//6831,Ossidi di Azoto,µg/m³,657,Lonato del Garda v.del Marchesino,180,BS,Lonato del Garda,N,01/01/1990,,5035536,615762,45.46375819,10.48078183,POINT (10.48078183 45.46375819)";
//            var noErrorFileTextBytes = Encoding.UTF8.GetBytes(noErrorFileText);
//            var noErrorFileTextStream = new MemoryStream(noErrorFileTextBytes);

//            // create expected value stream for comparison

//            yield return new object[]
//            {
//                noErrorFileTextStream, expectedAverageProvinceDataStream)
//            };
//        }
//    }
//}