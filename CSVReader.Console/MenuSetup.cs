using ConsoleMenu.Application;
using ConsoleMenu.Contracts;
using CSVReader.Core.Contracts.Services;

namespace CSVReader.Console
{
    public class MenuSetup
    {
        private readonly IFilePersistanceService _filePersistanceService;
        private readonly IConsoleMenuSelector _consoleMenuSelector;
        private readonly IConsoleMenuExecutor _consoleMenuExecutor;

        public MenuSetup(IFilePersistanceService filePersistanceService, IConsoleMenuSelector consoleMenuSelector, IConsoleMenuExecutor consoleMenuExecutor)
        {
            _filePersistanceService = filePersistanceService;
            _consoleMenuSelector = consoleMenuSelector;
            _consoleMenuExecutor = consoleMenuExecutor;
        }

        public async Task RunAsync()
        {
            var menu = new ConsoleMenuSetup(_consoleMenuSelector, _consoleMenuExecutor);

            menu.AddHandlerOption(1, "Initialize", "initialize");

            menu.AddOptionAsync(2, "Create readings", async () =>
            { await _filePersistanceService.CreateReadings(); });

            menu.AddOptionAsync(3, "Create consistent readings", async () =>
            { await _filePersistanceService.CreateConsistentReadings(); });

            menu.AddOptionAsync(4, "Create average province data", async () =>
            { await _filePersistanceService.CreateAverageProvinceDatas(); });

            menu.AddOptionAsync(5, "Create average province data output files", async () =>
            { await _filePersistanceService.CreateAverageProvinceDataOutputFiles(); });

            menu.AddOptionAsync(6, "Create consistent reading output files", async () =>
            { await _filePersistanceService.CreateConsistentReadingOutputFiles(); });

            menu.AddHandlerOption(7, "Export average province data files", "export-averageprovincedata");

            menu.AddHandlerOption(8, "Export consistent reading files", "export-consistentreading");

            menu.AddExitOption(9, "Exit");

            await menu.Run();
        }
    }
}
