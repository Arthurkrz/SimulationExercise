using ConsoleMenu.Contracts;
using Microsoft.IdentityModel.Tokens;
using SimulationExercise.Core.Contracts.Services;

namespace SimulationExercise.Services.Handlers
{
    public class ExportConsistentReadingHandler : IConsoleMenuHandler
    {
        private readonly IConsistentReadingExportService _exportService;

        public ExportConsistentReadingHandler(IConsistentReadingExportService exportService)
        {
            _exportService = exportService;
        }

        public string Key => "export-consistentreading";

        public async Task ExecuteAsync()
        {
            var loopPath = true;
            var path = "";

            while (loopPath)
            {
                Console.WriteLine("Insert the path where the output files will be exported:");
                path = Console.ReadLine();

                if (path.IsNullOrEmpty())
                {
                    Console.Clear();
                    Console.WriteLine("No path specified.\n");
                }

                loopPath = false;
            }

            await _exportService.Export(path!);
        }
    }
}
