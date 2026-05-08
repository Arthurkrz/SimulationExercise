using ConsoleMenu.Contracts;
using Microsoft.IdentityModel.Tokens;
using CSVReader.Core.Contracts.Services;

namespace CSVReader.Services.Handlers
{
    public class InitializeHandler : IConsoleMenuHandler
    {
        private readonly IInputFileService _inputFileService;

        public InitializeHandler(IInputFileService inputFileService)
        {
            _inputFileService = inputFileService;
        }

        public string Key => "initialize";

        public async Task ExecuteAsync()
        {
            var loopPath = true;
            var path = "";

            while (loopPath)
            {
                Console.WriteLine("Insert the path where the input files are located:");
                path = Console.ReadLine();

                if (path.IsNullOrEmpty())
                {
                    Console.Clear();
                    Console.WriteLine("No path specified.\n");
                }

                loopPath = false;
            }

            await _inputFileService.ProcessFilesAsync(path!);
        }
    }
}
