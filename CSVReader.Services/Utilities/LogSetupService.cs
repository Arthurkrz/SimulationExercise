using CSVReader.Core.Contracts.Services;

namespace CSVReader.Services.Utilities
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
            while (true)
            {
                Console.WriteLine("Insert the path where errors will be logged:");
                var path = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(path))
                {
                    Console.Clear();
                    Console.WriteLine("No path specified.\n");
                    continue;
                }

                var isValid = _filePersistanceService.LoggerConfiguration(path);

                if (!isValid)
                {
                    Console.Clear();
                    Console.WriteLine("Path not located.\n");
                    continue;
                }

                Console.Clear();
                break;
            }
        }
    }
}
