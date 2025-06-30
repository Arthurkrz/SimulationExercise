using SimulationExercise.Core.Contracts.Services;

namespace SimulationExercise.Services
{
    public class FilePersistanceService : IFilePersistanceService
    {
        private readonly IInputFileService _inputFileService;

        public FilePersistanceService(IInputFileService inputFileService)
        {
            _inputFileService = inputFileService;
        }

        public void Initialize(string inDirectoryPath) =>
            _inputFileService.ProcessFiles(inDirectoryPath);
    }
}
