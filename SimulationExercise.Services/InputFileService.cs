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
        private readonly ILogger<InputFileService> _logger;

        public InputFileService(IContextFactory contextFactory, 
                                IInputFileRepository inputFileRepository, 
                                ILogger<InputFileService> logger)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
            _logger = logger;
        }

        public void ProcessFiles(string inDirectoryPath)
        {
            using (IContext context = _contextFactory.Create())
            {
                try
                {
                    if (!Directory.Exists(inDirectoryPath))
                        Directory.CreateDirectory(inDirectoryPath);

                    var files = Directory.GetFiles(inDirectoryPath, "*.csv");
                    if (files.Length == 0) 
                        throw new ArgumentNullException(LogMessages.NOCSVFILESFOUND);

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

                        _inputFileRepository.Insert(inputFileInsertDTO, context);
                        SendToBackup(file, inDirectoryPath);
                    }

                    context.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        private void SendToBackup(string file, string inDirectoryPath)
        {
            string backupDirectory = Path.Combine(inDirectoryPath, "BACKUP");

            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            string destinationPath = Path.Combine(backupDirectory, Path.GetFileName(file));
            File.Move(file, destinationPath);
        }
    }
}