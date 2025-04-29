using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimulationExercise.Console;
using SimulationExercise.Core.Contracts;
using SimulationExercise.Core.Entities;
using SimulationExercise.IOC;

ServiceCollection services = new ServiceCollection();
DependencyInjection.InjectServices(services);
DependencyInjection.InjectValidators(services);
services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

using var serviceProvider = services.BuildServiceProvider();

var readingImportService = serviceProvider.GetRequiredService<IReadingImportService>();
var consistentReadingFactory = serviceProvider.GetRequiredService<IConsistentReadingFactory>();
var provinceDataListFactory = serviceProvider.GetRequiredService<IProvinceDataListFactory>();
var averageProvinceDataFactory = serviceProvider.GetRequiredService<IAverageProvinceDataFactory>();
var averageProvinceDataExportService = serviceProvider.GetRequiredService<IAverageProvinceDataExportService>();
var logger = serviceProvider.GetRequiredService<ILogger<App>>();

var app = new App(readingImportService, consistentReadingFactory, 
                  provinceDataListFactory, averageProvinceDataFactory, 
                  averageProvinceDataExportService, logger);

Console.WriteLine("Welcome! Readings import in CSV file and " +
                  "export of average province data to a new " +
                  "CSV file will occur in 5 steps.\n");

Console.WriteLine("1. Import readings from CSV file.");
Console.WriteLine("2. Create consistent readings.");
Console.WriteLine("3. Create province data list.");
Console.WriteLine("4. Create average province data.");
Console.WriteLine("5. Export average province data to a new CSV file.");

Console.WriteLine("\nPress any key to continue...");
Console.ReadKey();
Console.Clear();

Console.WriteLine("Step 1: Import readings from CSV file.\n");

ImportResult resultStep1 = null;
try
{
    resultStep1 = app.ImportReadings(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise\IN");

}
catch (Exception ex)
{
    Console.WriteLine("\nPress any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep1), ex.Message);
}

if (resultStep1.Errors.Count > 0)
{
    logger.LogWarning("Errors ocurred while importing readings! " +
                      "Check ErrorLog file for more information.\n");
}

if (resultStep1.Readings.Count == 0)
{
    logger.LogError("No readings have been imported!");
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep1));
}

string successMessageStep1 = resultStep1.Readings.Count == 1
    ? "1 reading imported successfully!"
    : $"{resultStep1.Readings.Count} readings imported successfully!";

logger.LogInformation(successMessageStep1);
Console.WriteLine("\nPress any key to continue...");
Console.ReadKey();
Console.Clear();

Console.WriteLine("Step 2: Create consistent readings.\n");

IList<Result<ConsistentReading>> resultStep2 = null;
try
{
    resultStep2 = app.CreateConsistentReadings(resultStep1.Readings);

}
catch (Exception ex)
{
    Console.WriteLine("\nPress any key to exit the program...");
    Console.ReadKey();
     throw new ArgumentException(nameof(resultStep2), ex.Message);
}

int successCountStep2 = 0;
int errorCountStep2 = 0;
IList<ConsistentReading> succesfullyCreatedCRs = 
    new List<ConsistentReading>();

foreach (var results in resultStep2)
{
    if (results.Success)
    {
        successCountStep2++;
        succesfullyCreatedCRs.Add(results.Value);
    }
    else
    {
        errorCountStep2++;
    }
}

if (errorCountStep2 > 0)
{
    logger.LogWarning("Errors ocurred while creating consistent readings! " +
                            "Check ErrorLog file for more information.");
}

if (successCountStep2 == 0)
{
    logger.LogError("No consistent readings have been created!");
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep2));
}

string successMessageStep2 = successCountStep2 == 1
    ? "1 consistent reading created successfully!\n"
    : $"{successCountStep2} consistent readings created successfully!\n";

logger.LogInformation(successMessageStep2);
Thread.Sleep(100);

Console.WriteLine("Press any key to continue...\n");
Console.ReadKey();
Console.Clear();

Console.WriteLine("Step 3: Create province data list.\n");

IList<ProvinceData> resultStep3 = null;
try
{
    resultStep3 = app.CreateProvinceDataList(succesfullyCreatedCRs);
}
catch
{
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep3));
}

if (resultStep3.Count == 0)
{
    logger.LogError("No province data have been created!");
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep3));
}

string successMessageStep3 = resultStep3.Count == 1
    ? "1 province data created successfully!\n"
    : $"{resultStep3.Count} province data created successfully!\n";

logger.LogInformation(successMessageStep3);
Thread.Sleep(100);

Console.WriteLine("Press any key to continue...");
Console.ReadKey();
Console.Clear();

Console.WriteLine("Step 4: Create average province data.\n");

IList<Result<AverageProvinceData>> resultStep4 = null;
try
{
    resultStep4 = app.CreateAverageProvinceData(resultStep3);
}
catch(Exception ex)
{
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep4));
}

int successCountStep4 = 0;
int errorCountStep4 = 0;
IList<AverageProvinceData> successfullyCreatedAPDs = 
    new List<AverageProvinceData>();

foreach (var results in resultStep4)
{
    if (results.Success)
    {
        successCountStep4++;
        successfullyCreatedAPDs.Add(results.Value);
    }

    else
    {
        errorCountStep4++;
    }
}

if (successCountStep4 == 0)
{
    logger.LogError("No average province data have been created!");
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(nameof(resultStep4));
}

if (errorCountStep4 > 0)
{
    logger.LogWarning("Errors ocurred while creating average province data! " +
                      "Check ErrorLog file for more information.");
}

string successMessageStep4 = resultStep4.Count == 1
    ? "1 average province data created successfully!\n"
    : $"{resultStep4.Count} average province data created succesfully!\n";

logger.LogInformation(successMessageStep4);
Thread.Sleep(100);

Console.WriteLine("Press any key to continue...");
Console.ReadKey();
Console.Clear();

Console.WriteLine("Step 5. Export average province data to a new CSV file.\n");
try
{
    app.ExportAverageProvinceData(@"C:\Users\PC\Documents\TechClass\SimulationExercise\SimulationExercise\OUT",
                                  DateTime.Now, successfullyCreatedAPDs);
}
catch(Exception ex)
{
    Console.WriteLine("Press any key to exit the program...");
    Console.ReadKey();
    throw new ArgumentException(ex.Message);
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Done!\n");
Console.ResetColor();