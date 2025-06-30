using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services
{
    public class InputFileService : IInputFileService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingService _readingService;
        private readonly ILogger<InputFileService> _logger;

        public InputFileService(IContextFactory contextFactory, 
                                IInputFileRepository inputFileRepository, 
                                IReadingService readingService,
                                ILogger<InputFileService> logger)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
            _readingService = readingService;
            _logger = logger;
        }

        public void ProcessFiles(string inDirectoryPath)
        {
            var files = LocateFiles(inDirectoryPath);

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileExtension = Path.GetExtension(file);
                var fileBytes = File.ReadAllBytes(file);

                if (fileBytes.Length == 0)
                {
                    _logger.LogError(LogMessages.EMPTYFILE);
                    _logger.LogInformation(LogMessages.CONTINUETONEXTFILE);
                    continue;
                }

                var inputFileInsertDTO = new InputFileInsertDTO
                    (fileName, fileBytes, fileExtension, Status.New);

                IList<InputFileGetDTO> inputFiles = new List<InputFileGetDTO>();

                using (IContext context = _contextFactory.Create())
                {
                    _inputFileRepository.Insert(inputFileInsertDTO, context);
                    SendToBackup(file);

                    inputFiles = _inputFileRepository.GetByStatus(Status.New, context);
                }

                _readingService.ProcessInputFiles(inputFiles.First());
            }
        }

        private string[] LocateFiles(string inDirectoryPath)
        {
            if (!Directory.Exists(inDirectoryPath))
                Directory.CreateDirectory(inDirectoryPath);

            var files = Directory.GetFiles(inDirectoryPath);

            if (files.Length == 0) throw new ArgumentNullException
                                     (LogMessages.NOCSVFILESFOUND);
            return files;
        }

        private void SendToBackup(string filePath)
        {
            string backupPath = Path.Combine(filePath, "BACKUP");
            File.Move(filePath, backupPath);
        }
    }
}