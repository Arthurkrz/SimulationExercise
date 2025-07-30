using FileHelpers;
using SimulationExercise.Core.CSVDTOs;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using System.Text;

namespace SimulationExercise.Tests.Integration.ObjectGenerators
{
    public class StreamData
    {
        public static IEnumerable<object[]> ValidStreamGenerator()
        {
            var engine = new FileHelperEngine<ConsistentReadingExportDTO>();

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

            var bytesInputText = Encoding.UTF8.GetBytes(inputText);
            var streamInput = new MemoryStream(bytesInputText);

            string outputText = $@"SensorId,SensorTypeName,Unit,Value,Province,City,IsHistoric,DaysOfMeasure,UtmNord,UtmEst,Latitude,Longitude
12691,Arsenico,ng_m3,383,VA,Varese,False,{(int)(SystemTime.Now().Date - new DateTime(2008,4,1).Date).TotalDays},5073728,486035,45.8169745,8.82024911
5712,Ozono,µg_m3,138,MI,Inzago,True,{(int)(new DateTime(2018,1,1).Date - new DateTime(2001,02,24).Date).TotalDays},5043030,538012,45.53976956,9.48689669
20488,Ozono,µg_m3,279,MI,Erba,False,{(int)(SystemTime.Now().Date - new DateTime(2020,10,22).Date).TotalDays},5072803,517232,45.8085738,9.2217792
10043,Arsenico,ng_m3,215,VA,Ferno,False,{(int)(SystemTime.Now().Date - new DateTime(2006,11,29).Date).TotalDays},5051773,481053,45.61924753,8.75697656
6342,Ossidi di Azoto,mg_m3,141,BS,Pero,True,{(int)(new DateTime(2018,07,30).Date - new DateTime(1986,12,10).Date).TotalDays},5039595,507028,45.50985564,9.08997419
6665,Ossidi di Azoto,mg_m3,369,BS,Cantù,False,{(int)(SystemTime.Now().Date - new DateTime(2005,01,17).Date).TotalDays},5064150,509783,45.73083728,9.12573936
10270,Ozono,µg_m3,221,MI,Darfo Boario Terme,True,{(int)(new DateTime(2023,07,12).Date - new DateTime(2007,1,1).Date).TotalDays},5080789,591371,45.87460256,10.17736553
5507,Biossido di Azoto,µg_m3,139,CO,Sesto San Giovanni,False,{(int)(SystemTime.Now().Date - new DateTime(1980,12,18).Date).TotalDays},5042386,518435,45.53476819,9.23610903
10035,Benzene,µg_m3,74,CO,Cornale,False,{(int)(SystemTime.Now().Date - new DateTime(2006,2,2).Date).TotalDays},4987406,493238,45.04008561,8.91415717
6831,Ossidi di Azoto,mg_m3,180,BS,Lonato del Garda,False,{(int)(SystemTime.Now().Date - new DateTime(1990,1,1).Date).TotalDays},5035536,615762,45.46375819,10.48078183";

            var bytesOutputText = Encoding.UTF8.GetBytes(outputText);

            yield return new object[]
            {
                streamInput,

                new List<string>
                {
                    "IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location",
                    "12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)",
                    "5712,Ozono,µg/m³,510,Inzago v.le Gramsci,138,MI,Inzago,S,24/02/2001,01/01/2018,5043030,538012,45.53976956,9.48689669,POINT (9.48689669 45.53976956)",
                    "20488,Ozono,µg/m³,564,Erba v. Battisti,279,MI,Erba,N,22/10/2020,,5072803,517232,45.8085738,9.2217792,POINT (9.2217792 45.8085738)",
                    "10043,Arsenico,ng/m³,687,Ferno v.Di Dio,215,VA,Ferno,N,29/11/2006,,5051773,481053,45.61924753,8.75697656,POINT (8.75697656 45.61924753)",
                    "6342,Ossidi di Azoto,mg/m³,515,Pero SS Sempione,141,BS,Pero,S,10/12/1986,30/07/2018,5039595,507028,45.50985564,9.08997419,POINT (9.08997419 45.50985564)",
                    "6665,Ossidi di Azoto,mg/m³,565,Cantù v.Meucci,369,BS,Cantù,N,17/01/2005,,5064150,509783,45.73083728,9.12573936,POINT (9.12573936 45.73083728)",
                    "10270,Ozono,µg/m³,655,Darfo Boario Terme v. De Amicis,221,MI,Darfo Boario Terme,S,01/01/2007,12/07/2023,5080789,591371,45.87460256,10.17736553,POINT (10.17736553 45.87460256)",
                    "5507,Biossido di Azoto,µg/m³,504,Sesto San Giovanni v. Cesare da Sesto,139,CO,Sesto San Giovanni,N,18/12/1980,,5042386,518435,45.53476819,9.23610903,POINT (9.23610903 45.53476819)",
                    "10035,Benzene,µg/m³,672,Cornale v.Libertà,74,CO,Cornale,N,02/02/2006,,4987406,493238,45.04008561,8.91415717,POINT (8.91415717 45.04008561)",
                    "6831,Ossidi di Azoto,mg/m³,657,Lonato del Garda v.del Marchesino,180,BS,Lonato del Garda,N,01/01/1990,,5035536,615762,45.46375819,10.48078183,POINT (10.48078183 45.46375819)"
                },

                new List<ReadingGetDTO>
                {
                    new ReadingGetDTO(1, 1, 12691, "Arsenico", "ng/m³", 560, "Varese v.Copelli", 383, "VA", "Varese", false, new DateTime(2008, 4, 1), SystemTime.Now().Date, 5073728, 486035, "45.8169745", "8.82024911", Status.Success),
                    new ReadingGetDTO(2, 1, 5712, "Ozono", "µg/m³", 510, "Inzago v.le Gramsci", 138, "MI", "Inzago", true, new DateTime(2001, 2, 24), new DateTime(2018, 1, 1), 5043030, 538012, "45.53976956", "9.48689669", Status.Success),
                    new ReadingGetDTO(3, 1, 20488, "Ozono", "µg/m³", 564, "Erba v. Battisti", 279, "MI", "Erba", false, new DateTime(2020, 10, 22), SystemTime.Now().Date, 5072803, 517232, "45.8085738", "9.2217792", Status.Success),
                    new ReadingGetDTO(4, 1, 10043, "Arsenico", "ng/m³", 687, "Ferno v.Di Dio", 215, "VA", "Ferno", false, new DateTime(2006, 11, 29), SystemTime.Now().Date, 5051773, 481053, "45.61924753", "8.75697656", Status.Success),
                    new ReadingGetDTO(5, 1, 6342, "Ossidi di Azoto", "mg/m³", 515, "Pero SS Sempione", 141, "BS", "Pero", true, new DateTime(1986, 12, 10), new DateTime(2018, 7, 30), 5039595, 507028, "45.50985564", "9.08997419", Status.Success),
                    new ReadingGetDTO(6, 1, 6665, "Ossidi di Azoto", "mg/m³", 565, "Cantù v.Meucci", 369, "BS", "Cantù", false, new DateTime(2005, 1, 17), SystemTime.Now().Date, 5064150, 509783, "45.73083728", "9.12573936", Status.Success),
                    new ReadingGetDTO(7, 1, 10270, "Ozono", "µg/m³", 655, "Darfo Boario Terme v. De Amicis", 221, "MI", "Darfo Boario Terme", true, new DateTime(2007, 1, 1), new DateTime(2023, 7, 12), 5080789, 591371, "45.87460256", "10.17736553", Status.Success),
                    new ReadingGetDTO(8, 1, 5507, "Biossido di Azoto", "µg/m³", 504, "Sesto San Giovanni v. Cesare da Sesto", 139, "CO", "Sesto San Giovanni", false, new DateTime(1980, 12, 18), SystemTime.Now().Date, 5042386, 518435, "45.53476819", "9.23610903", Status.Success),
                    new ReadingGetDTO(9, 1, 10035, "Benzene", "µg/m³", 672, "Cornale v.Libertà", 74, "CO", "Cornale", false, new DateTime(2006, 2, 2), SystemTime.Now().Date, 4987406, 493238, "45.04008561", "8.91415717", Status.Success),
                    new ReadingGetDTO(10, 1, 6831, "Ossidi di Azoto", "mg/m³", 657, "Lonato del Garda v.del Marchesino", 180, "BS", "Lonato del Garda", false, new DateTime(1990, 1, 1), SystemTime.Now().Date, 5035536, 615762, "45.46375819", "10.48078183", Status.Success),
                },

                new List<ConsistentReadingGetDTO>
                {
                    new ConsistentReadingGetDTO(10, 1, 12691, "Arsenico", Unit.ng_m3, 383, "VA", "Varese", false, (int)(SystemTime.Now().Date - new DateTime(2008,4,1).Date).TotalDays, 5073728, 486035, "45.8169745", "8.82024911", Status.Success),
                    new ConsistentReadingGetDTO(9, 2, 5712, "Ozono", Unit.µg_m3, 138, "MI", "Inzago", true, (int)(new DateTime(2018,1,1).Date - new DateTime(2001,02,24).Date).TotalDays, 5043030, 538012, "45.53976956", "9.48689669", Status.Success),
                    new ConsistentReadingGetDTO(8, 6, 6665, "Ossidi di Azoto", Unit.mg_m3, 369, "BS", "Cantù", false, (int)(SystemTime.Now().Date - new DateTime(2005,01,17).Date).TotalDays, 5064150, 509783, "45.73083728", "9.12573936", Status.Success),
                    new ConsistentReadingGetDTO(7, 5, 6342, "Ossidi di Azoto", Unit.mg_m3, 141, "BS", "Pero", true, (int)(new DateTime(2018,07,30).Date - new DateTime(1986,12,10).Date).TotalDays, 5039595, 507028, "45.50985564", "9.08997419", Status.Success),
                    new ConsistentReadingGetDTO(6, 4, 10043, "Arsenico", Unit.ng_m3, 215, "VA", "Ferno", false, (int)(SystemTime.Now().Date - new DateTime(2006,11,29).Date).TotalDays, 5051773, 481053, "45.61924753", "8.75697656", Status.Success),
                    new ConsistentReadingGetDTO(5, 3, 20488, "Ozono", Unit.µg_m3, 279, "MI", "Erba", false, (int)(SystemTime.Now().Date - new DateTime(2020,10,22).Date).TotalDays, 5072803, 517232, "45.8085738", "9.2217792", Status.Success),
                    new ConsistentReadingGetDTO(4, 10, 6831, "Ossidi di Azoto", Unit.mg_m3, 180, "BS", "Lonato del Garda", false, (int)(SystemTime.Now().Date - new DateTime(1990,1,1).Date).TotalDays, 5035536, 615762, "45.46375819", "10.48078183", Status.Success),
                    new ConsistentReadingGetDTO(3, 9, 10035, "Benzene", Unit.µg_m3, 74, "CO", "Cornale", false, (int)(SystemTime.Now().Date - new DateTime(2006,2,2).Date).TotalDays, 4987406, 493238, "45.04008561", "8.91415717", Status.Success),
                    new ConsistentReadingGetDTO(2, 8, 5507, "Biossido di Azoto", Unit.µg_m3, 139, "CO", "Sesto San Giovanni", false, (int)(SystemTime.Now().Date - new DateTime(1980,12,18).Date).TotalDays, 5042386, 518435, "45.53476819", "9.23610903", Status.Success),
                    new ConsistentReadingGetDTO(1, 7, 10270, "Ozono", Unit.µg_m3, 221, "MI", "Darfo Boario Terme", true, (int)(new DateTime(2023,07,12).Date - new DateTime(2007,1,1).Date).TotalDays, 5080789, 591371, "45.87460256", "10.17736553", Status.Success)
                },

                new List<string>
                {
                    $"SensorId,SensorTypeName,Unit,Value,Province,City,IsHistoric,DaysOfMeasure,UtmNord,UtmEst,Latitude,Longitude",
                    $"12691,Arsenico,ng_m3,383,VA,Varese,False,{(int)(SystemTime.Now().Date - new DateTime(2008,4,1).Date).TotalDays},5073728,486035,45.8169745,8.82024911",
                    $"5712,Ozono,µg_m3,138,MI,Inzago,True,{(int)(new DateTime(2018,1,1).Date - new DateTime(2001,02,24).Date).TotalDays},5043030,538012,45.53976956,9.48689669",
                    $"20488,Ozono,µg_m3,279,MI,Erba,False,{(int)(SystemTime.Now().Date - new DateTime(2020,10,22).Date).TotalDays},5072803,517232,45.8085738,9.2217792",
                    $"10043,Arsenico,ng_m3,215,VA,Ferno,False,{(int)(SystemTime.Now().Date - new DateTime(2006,11,29).Date).TotalDays},5051773,481053,45.61924753,8.75697656",
                    $"6342,Ossidi di Azoto,mg_m3,141,BS,Pero,True,{(int)(new DateTime(2018,07,30).Date - new DateTime(1986,12,10).Date).TotalDays},5039595,507028,45.50985564,9.08997419",
                    $"6665,Ossidi di Azoto,mg_m3,369,BS,Cantù,False,{(int)(SystemTime.Now().Date - new DateTime(2005,01,17).Date).TotalDays},5064150,509783,45.73083728,9.12573936",
                    $"10270,Ozono,µg_m3,221,MI,Darfo Boario Terme,True,{(int)(new DateTime(2023,07,12).Date - new DateTime(2007,1,1).Date).TotalDays},5080789,591371,45.87460256,10.17736553",
                    $"5507,Biossido di Azoto,µg_m3,139,CO,Sesto San Giovanni,False,{(int)(SystemTime.Now().Date - new DateTime(1980,12,18).Date).TotalDays},5042386,518435,45.53476819,9.23610903",
                    $"10035,Benzene,µg_m3,74,CO,Cornale,False,{(int)(SystemTime.Now().Date - new DateTime(2006,2,2).Date).TotalDays},4987406,493238,45.04008561,8.91415717",
                    $"6831,Ossidi di Azoto,mg_m3,180,BS,Lonato del Garda,False,{(int)(SystemTime.Now().Date - new DateTime(1990,1,1).Date).TotalDays},5035536,615762,45.46375819,10.48078183"
                }
            };
        }

        public static IEnumerable<object[]> InvalidStreamGenerator()
        {
            string inputWithErrorsText = $@"IdSensore,NomeTipoSensore,UnitaMisura,Idstazione,NomeStazione,Quota,Provincia,Comune,Storico,DataStart,DataStop,Utm_Nord,UTM_Est,lat,lng,Location
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
ERROR,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,ERROR,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,ERROR,VA,Varese,N,01/04/2008,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,ERROR,,5073728,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,ERROR,486035,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
12691,Arsenico,ng/m³,560,Varese v.Copelli,383,VA,Varese,N,01/04/2008,,5073728,ERROR,45.8169745,8.82024911,POINT (8.82024911 45.8169745)
-1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ERROR,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,0,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,-1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,-1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,µg/m³,1,Station Name,1,Province,,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,01/01/1967,{SystemTime.Now():dd/MM/yyyy},1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now():dd/MM/yyyy},{SystemTime.Now().AddYears(-1):dd/MM/yyyy},1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,0,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,-1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,0,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,-1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{SystemTime.Now().AddYears(-1):dd/MM/yyyy},,1,1,Latitude,,
0,,,0,,-1,,,N,01/01/1967,01/01/1966,0,0,,,
-1,,ERROR,-1,,-1,,,N,01/01/1967,01/01/1966,-1,-1,,,";

            var bytesInputWithErrorsText = Encoding.UTF8.GetBytes(inputWithErrorsText);
            var streamInputWithErrors = new MemoryStream(bytesInputWithErrorsText);

            var expectedErrors = new List<string>
            {
                "[ERR] Errors found in Input File! (0)",
                "[ERR] Line: 2 Column: 1. No value found for the value type field: '<IdSensore>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "[ERR] Line: 3 Column: 22. No value found for the value type field: '<Idstazione>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "[ERR] Line: 4 Column: 43. No value found for the value type field: '<Quota>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "[ERR] Line: 5 Column: 59. No value found for the value type field: '<DataStart>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "[ERR] Line: 6 Column: 71. No value found for the value type field: '<Utm_Nord>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "[ERR] Line: 7 Column: 79. No value found for the value type field: '<UTM_Est>k__BackingField' Class: 'ReadingDTO'. \r\nYou must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.",
                "[ERR] Error Converting 'ERROR' to type: 'Int64'. ",
                "[ERR] Error Converting 'ERROR' to type: 'Int64'. ",
                "[ERR] Error Converting 'ERROR' to type: 'Int32'. ",
                "[ERR] Error Converting 'ERROR' to type: 'DateTime'.  There are less chars in the Input String than in the Format string: 'dd/MM/yyyy'",
                "[ERR] Error Converting 'ERROR' to type: 'Int32'. ",
                "[ERR] Error Converting 'ERROR' to type: 'Int32'. ",
                "[ERR] Errors found in Reading! (0)",
                "[ERR] Sensor ID less or equal to 0.",
                "[ERR] Errors found in Reading! (1)",
                "[ERR] Null or empty sensor name.",
                "[ERR] Errors found in Reading! (3)",
                "[ERR] Unit not supported.",
                "[ERR] Errors found in Reading! (4)",
                "[ERR] Station ID less or equal to 0.",
                "[ERR] Errors found in Reading! (5)",
                "[ERR] Station ID less or equal to 0.",
                "[ERR] Errors found in Reading! (7)",
                "[ERR] Null or empty station name.",
                "[ERR] Errors found in Reading! (8)",
                "[ERR] Negative value.",
                "[ERR] Errors found in Reading! (9)",
                "[ERR] Null or empty province name.",
                "[ERR] Errors found in Reading! (12)",
                "[ERR] Null or empty city name.",
                "[ERR] Errors found in Reading! (13)",
                "[ERR] Start date is before the possible minimum.",
                "[ERR] Errors found in Reading! (14)",
                "[ERR] Stop date is before start date.",
                "[ERR] Errors found in Reading! (15)",
                "[ERR] UTMNord less or equal to 0.",
                "[ERR] Errors found in Reading! (16)",
                "[ERR] UTMNord less or equal to 0.",
                "[ERR] Errors found in Reading! (17)",
                "[ERR] UTMEst less or equal to 0.",
                "[ERR] Errors found in Reading! (18)",
                "[ERR] UTMEst less or equal to 0.",
                "[ERR] Errors found in Reading! (20)",
                "[ERR] Null or empty latitude.",
                "[ERR] Errors found in Reading! (22)",
                "[ERR] Null or empty longitude.",
                "[ERR] Errors found in Reading! (23)",
                "[ERR] Sensor ID less or equal to 0.",
                "[ERR] Null or empty sensor name.",
                "[ERR] Unit not supported.",
                "[ERR] Station ID less or equal to 0.",
                "[ERR] Null or empty station name.",
                "[ERR] Negative value.",
                "[ERR] Null or empty province name.",
                "[ERR] Null or empty city name.",
                "[ERR] Start date is before the possible minimum.",
                "[ERR] Stop date is before start date.",
                "[ERR] UTMNord less or equal to 0.",
                "[ERR] UTMEst less or equal to 0.",
                "[ERR] Null or empty latitude.",
                "[ERR] Null or empty longitude.",
                "[ERR] Errors found in Reading! (24)",
                "[ERR] Sensor ID less or equal to 0.",
                "[ERR] Null or empty sensor name.",
                "[ERR] Unit not supported.",
                "[ERR] Station ID less or equal to 0.",
                "[ERR] Null or empty station name.",
                "[ERR] Negative value.",
                "[ERR] Null or empty province name.",
                "[ERR] Null or empty city name.",
                "[ERR] Start date is before the possible minimum.",
                "[ERR] Stop date is before start date.",
                "[ERR] UTMNord less or equal to 0.",
                "[ERR] UTMEst less or equal to 0.",
                "[ERR] Null or empty latitude.",
                "[ERR] Null or empty longitude."
            };

            var expectedReadings = new List<ReadingGetDTO>
            {
                new ReadingGetDTO(2, 1, -1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(3, 1, 1, "", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(4, 1, 1, "Sensor Name", "ERROR", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(5, 1, 1, "Sensor Name", "ng/m³", 0, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(6, 1, 1, "Sensor Name", "ng/m³", -1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(7, 1, 1, "Sensor Name", "ng/m³", 1, "", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(8, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", -1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(9, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(10, 1, 1, "Sensor Name", "µg/m³", 1, "Station Name", 1, "Province", "", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(11, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, new DateTime(1967, 1, 1), SystemTime.Now().Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(12, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().Date, SystemTime.Now().AddYears(-1).Date, 1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(13, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 0, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(14, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, -1, 1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(15, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 0, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(16, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, -1, "Latitude", "Longitude", Status.Error),
                new ReadingGetDTO(17, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "", "Longitude", Status.Error),
                new ReadingGetDTO(18, 1, 1, "Sensor Name", "ng/m³", 1, "Station Name", 1, "Province", "City", true, SystemTime.Now().AddYears(-1).Date, SystemTime.Now().Date, 1, 1, "Latitude", "", Status.Error),
                new ReadingGetDTO(19, 1, 0, "", "", 0, "", -1, "", "", false, new DateTime(1967, 1, 1), new DateTime(1966, 1, 1), 0, 0, "", "", Status.Error),
                new ReadingGetDTO(20, 1, -1, "", "ERROR", -1, "", -1, "", "", false, new DateTime(1967, 1, 1), new DateTime(1966, 1, 1), -1, -1, "", "", Status.Error)
            };

            yield return new object[]
            {
                streamInputWithErrors, expectedErrors, expectedReadings
            };
        }
    }
}
