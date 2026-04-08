using Microsoft.IdentityModel.Tokens;
using SimulationExercise.Core.Contracts.Services;

namespace SimulationExercise.Services.Utilities
{
    public class LogSetupService : ILogSetupService
    {
        private readonly IFilePersistanceService _filePersistanceService;

        public LogSetupService(IFilePersistanceService filePersistanceService)
        {
            _filePersistanceService = filePersistanceService;
        }

        public void Configure()
        {
            var path = "";
            var loopPathInvalid = true;
            var loopPathEmpty = true;
            var loopPathNotLocated = true;

            while (loopPathInvalid)
            {
                while (loopPathEmpty)
                {
                    Console.WriteLine("Insert the path where errors will be logged:");
                    path = Console.ReadLine();

                    if (path.IsNullOrEmpty())
                    {
                        Console.Clear();
                        Console.WriteLine("No path specified.\n");
                    }

                    else loopPathEmpty = false;
                }

                while (loopPathNotLocated)
                {
                    var isValid = _filePersistanceService.LoggerConfiguration(path!);

                    if (!isValid)
                    {
                        Console.Clear();
                        Console.WriteLine("Path not located.\n");
                    }

                    else loopPathNotLocated = false;
                }

                loopPathInvalid = false;
            }
        }
    }
}
