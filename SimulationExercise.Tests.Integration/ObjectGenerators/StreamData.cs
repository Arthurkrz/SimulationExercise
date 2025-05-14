using System.Text;

namespace SimulationExercise.Tests.Integration.ObjectGenerators
{
    public class StreamData
    {
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

            string expectedOutputText = $@"VA,Arsenico,299,ng_m3,{(int)(((DateTime.Now.Date - new DateTime(2008, 04, 01).Date).TotalDays + (DateTime.Now.Date - new DateTime(2006, 11, 29).Date).TotalDays) / 2)}
MI,Ozono,212.67,µg_m3,{(int)(((new DateTime(2018, 1, 1).Date - new DateTime(2001, 02, 24).Date).TotalDays + (DateTime.Now.Date - new DateTime(2020, 10, 22).Date).TotalDays + (new DateTime(2023, 7, 12).Date - new DateTime(2007, 1, 1).Date).TotalDays) / 3)}
BS,Ossidi di Azoto,230,mg_m3,{(int)(((new DateTime(2018, 7, 30).Date - new DateTime(1986, 12, 10).Date).TotalDays + (DateTime.Now.Date - new DateTime(2005, 1, 17).Date).TotalDays + (DateTime.Now.Date - new DateTime(1990, 1, 1).Date).TotalDays) / 3)}
CO,Biossido di Azoto,139,µg_m3,{(int)((DateTime.Now.Date - new DateTime(1980, 12, 18).Date).TotalDays)}
CO,Benzene,74,µg_m3,{(int)((DateTime.Now.Date - new DateTime(2006, 2, 2).Date).TotalDays)}";

            var bytesInputText = Encoding.UTF8.GetBytes(inputText);
            var streamInput = new MemoryStream(bytesInputText);

            yield return new object[]
            {
                streamInput, expectedOutputText
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
-1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ERROR,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,0,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,-1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,-1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,mg/m³,1,Station Name,1,,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,mg/m³,1,Station Name,1,Province,,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,µg/m³,1,Station Name,1,Province,,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,01/01/1967,{DateTime.Now:dd/MM/yyyy},1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now:dd/MM/yyyy},{DateTime.Now.AddYears(-1):dd/MM/yyyy},1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,0,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,-1,1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,0,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,-1,Latitude,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,,Longitude,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,,
1,Sensor Name,ng/m³,1,Station Name,1,Province,City,S,{DateTime.Now.AddYears(-1):dd/MM/yyyy},,1,1,Latitude,,
0,,,0,,-1,,,N,01/01/1967,01/01/1966,0,0,,,
-1,,ERROR,-1,,-1,,,N,01/01/1967,01/01/1966,-1,-1,,,";

            string expectedErrorsText = @"[ERR] Errors found! (0)
[ERR] Line: 2 Column: 1. No value found for the value type field: '<IdSensore>k__BackingField' Class: 'ReadingDTO'. 
You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.
[ERR] Line: 3 Column: 22. No value found for the value type field: '<Idstazione>k__BackingField' Class: 'ReadingDTO'. 
You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.
[ERR] Line: 4 Column: 43. No value found for the value type field: '<Quota>k__BackingField' Class: 'ReadingDTO'. 
You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.
[ERR] Line: 5 Column: 59. No value found for the value type field: '<DataStart>k__BackingField' Class: 'ReadingDTO'. 
You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.
[ERR] Line: 6 Column: 71. No value found for the value type field: '<Utm_Nord>k__BackingField' Class: 'ReadingDTO'. 
You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.
[ERR] Line: 7 Column: 79. No value found for the value type field: '<UTM_Est>k__BackingField' Class: 'ReadingDTO'. 
You must use the [FieldNullValue] attribute because this is a value type and can't be null or use a Nullable Type instead of the current type.
[ERR] Error Converting 'ERROR' to type: 'Int64'. 
[ERR] Error Converting 'ERROR' to type: 'Int64'. 
[ERR] Error Converting 'ERROR' to type: 'Int32'. 
[ERR] Error Converting 'ERROR' to type: 'DateTime'.  There are less chars in the Input String than in the Format string: 'dd/MM/yyyy'
[ERR] Error Converting 'ERROR' to type: 'Int32'. 
[ERR] Error Converting 'ERROR' to type: 'Int32'. 
[ERR] Errors found! (1)
[ERR] Sensor ID less or equal to 0.
[ERR] Errors found! (2)
[ERR] Null or empty sensor name.
[ERR] Errors found! (3)
[ERR] Null or empty sensor name.
[ERR] Errors found! (4)
[ERR] Unit not supported.
[ERR] Errors found! (5)
[ERR] Station ID less or equal to 0.
[ERR] Errors found! (6)
[ERR] Station ID less or equal to 0.
[ERR] Errors found! (7)
[ERR] Null or empty station name.
[ERR] Errors found! (8)
[ERR] Null or empty station name.
[ERR] Errors found! (9)
[ERR] Negative value.
[ERR] Errors found! (10)
[ERR] Null or empty province name.
[ERR] Errors found! (11)
[ERR] Null or empty province name.
[ERR] Errors found! (12)
[ERR] Null or empty city name.
[ERR] Errors found! (13)
[ERR] Null or empty city name.
[ERR] Errors found! (14)
[ERR] Start date is before the possible minimum.
[ERR] Errors found! (15)
[ERR] Stop date is before start date.
[ERR] Errors found! (16)
[ERR] UTMNord less or equal to 0.
[ERR] Errors found! (17)
[ERR] UTMNord less or equal to 0.
[ERR] Errors found! (18)
[ERR] UTMEst less or equal to 0.
[ERR] Errors found! (19)
[ERR] UTMEst less or equal to 0.
[ERR] Errors found! (20)
[ERR] Null or empty latitude.
[ERR] Errors found! (21)
[ERR] Null or empty latitude.
[ERR] Errors found! (22)
[ERR] Null or empty longitude.
[ERR] Errors found! (23)
[ERR] Null or empty longitude.
[ERR] Errors found! (24)
[ERR] Sensor ID less or equal to 0.
[ERR] Null or empty sensor name.
[ERR] Unit not supported.
[ERR] Station ID less or equal to 0.
[ERR] Null or empty station name.
[ERR] Negative value.
[ERR] Null or empty province name.
[ERR] Null or empty city name.
[ERR] Start date is before the possible minimum.
[ERR] Stop date is before start date.
[ERR] UTMNord less or equal to 0.
[ERR] UTMEst less or equal to 0.
[ERR] Null or empty latitude.
[ERR] Null or empty longitude.
[ERR] Errors found! (25)
[ERR] Sensor ID less or equal to 0.
[ERR] Null or empty sensor name.
[ERR] Unit not supported.
[ERR] Station ID less or equal to 0.
[ERR] Null or empty station name.
[ERR] Negative value.
[ERR] Null or empty province name.
[ERR] Null or empty city name.
[ERR] Start date is before the possible minimum.
[ERR] Stop date is before start date.
[ERR] UTMNord less or equal to 0.
[ERR] UTMEst less or equal to 0.
[ERR] Null or empty latitude.
[ERR] Null or empty longitude.
";

            var bytesInputWithErrorsText = Encoding.UTF8.GetBytes(inputWithErrorsText);
            var streamInputWithErrors = new MemoryStream(bytesInputWithErrorsText);

            yield return new object[]
            {
                streamInputWithErrors, expectedErrorsText
            };
        }
    }
}
