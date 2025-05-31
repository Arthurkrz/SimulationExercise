using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Services
{
    public class MessageGenerator
    {
        private readonly ILogger<FileProcessingService> _logger;

        public MessageGenerator(ILogger<FileProcessingService> logger)
        {
            _logger = logger;
        }

        public static string ExportDirectoryPathGeneratorAndLoggerConfiguration(string baseOutPath)
        {
            string specificReadingsAndErrorsDirectoryName =
                SystemTime.Now().ToString("yyyyMMdd_HHmmss");

            string fullFolderPath = Path.Combine(baseOutPath,
                            specificReadingsAndErrorsDirectoryName);

            Directory.CreateDirectory(fullFolderPath);

            string noErrorsFilePath = Path.Combine(fullFolderPath,
                                                   "AverageProvinceData.csv");

            string errorsFilePath = Path.Combine(fullFolderPath,
                                                 "Errors.log");

            LogPathHolder.ErrorLogPath = errorsFilePath;

            return noErrorsFilePath;
        }

        public void ExportMessage() =>
            _logger.LogInformation("Exporting average province data...");

        private void ContinueToNextFileMessage() =>
            _logger.LogInformation("Continuing to next file (if exists)...\n");

        public void ObjectCreationMessage(Type objectType, int count)
        {
            string successMessage = count == 1 ? $"1 {objectType.Name} created successfully!"
                : $"{count} {objectType.Name + "s"} created successfully!";

            _logger.LogInformation(successMessage);
        }

        public void ObjectErrorMessage(List<string> errorList)
        {
            int errorGroupNumber = 0;

            _logger.LogError($"Errors found! ({errorGroupNumber})");
            foreach (var errors in errorList)
            {
                _logger.LogError(errors);
                continue;
            }

            errorGroupNumber++;
        }

        public void NoObjectCreationMessage(Type objectType)
        {
            if (objectType == typeof(Reading))
                _logger.LogError("No readings have been imoprted!");

            _logger.LogError($"No {objectType.Name + "s"} have been created!");

            ContinueToNextFileMessage();
        }

        public void ObjectManipulationMessage(Type objectType)
        {
            if (objectType == typeof(Reading))
                _logger.LogInformation("Importing readings...");

            _logger.LogInformation($"Creating {objectType.Name + "s"}...");
        }
    }
}
