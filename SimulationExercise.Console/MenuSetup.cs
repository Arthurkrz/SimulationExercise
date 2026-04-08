using ConsoleMenu.Application;
using SimulationExercise.Core.Contracts.Services;

namespace SimulationExercise.Console
{
    public class MenuSetup
    {
        private readonly IFilePersistanceService _filePersistanceService;

        public MenuSetup(IFilePersistanceService filePersistanceService)
        {
            _filePersistanceService = filePersistanceService;
        }

        public async Task RunAsync()
        {
            var menu = new ConsoleMenuSetup();

            menu.AddHandlerOption(1, "Initialize", "initialize");

            menu.AddOptionAsync(2, "Create readings", async () =>
            { await _filePersistanceService.CreateReadings(); });

            menu.AddOptionAsync(3, "Create consistent readings", async () =>
            { await _filePersistanceService.CreateConsistentReadings(); });

            menu.AddOptionAsync(4, "Create average province data", async () =>
            { await _filePersistanceService.CreateAverageProvinceDatas(); });

            menu.AddOptionAsync(5, "Create average province data output files", async () =>
            { await _filePersistanceService.CreateAverageProvinceDataOutputFiles(); });

            menu.AddOptionAsync(6, "Create average province data output files", async () =>
            { await _filePersistanceService.CreateAverageProvinceDataOutputFiles(); });

            menu.AddHandlerOption(7, "Export average province data files", "export-averageprovincedata");

            menu.AddHandlerOption(8, "Export consistent reading files", "export-consistentreading");

            menu.AddExitOption(9, "Exit");

            await menu.Run();
        }
    }
}
