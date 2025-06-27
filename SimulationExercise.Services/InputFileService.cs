using FileHelpers;
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

        public InputFileService(IContextFactory contextFactory, IInputFileRepository inputFileRepository)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
        }

        public void ProcessFiles(string inDirectoryPath)
        {
            var files = LocateFiles(inDirectoryPath);

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileExtension = Path.GetExtension(file);
                var fileBytes = File.ReadAllBytes(file);

                List<string> errors = FileValidation(file, fileBytes);

                if (!errors.Any())
                {
                    var inputFileInsertDTO = new InputFileInsertDTO
                        (fileName, fileBytes, fileExtension, Status.New);

                    InsertAndUpdateFile(file, inputFileInsertDTO, errors);
                }

                var errorInputFileInsertDTO = new InputFileInsertDTO
                    (file, fileBytes, fileExtension, Status.Error);
                
                InsertAndUpdateFile(file, errorInputFileInsertDTO, errors);
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

        private List<string> FileValidation(string file, byte[] bytes)
        {
            List<string> inputFileErrors = new List<string>();

            var engine = new FileHelperEngine<ReadingDTO>();
            engine.ErrorMode = ErrorMode.SaveAndContinue;

            if (bytes.Length == 0) 
                return new List<string> { LogMessages.EMPTYFILE };

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    var currentHeader = reader.ReadLine().Replace(" ", "").TrimEnd(';');
                    var header = engine.GetFileHeader();

                    if (!header.Equals(currentHeader, StringComparison.OrdinalIgnoreCase))
                        return new List<string> { LogMessages.INVALIDHEADER };

                    var engineRecords = engine.ReadStream(reader);
                    var engineErrors = engine.ErrorManager.Errors;

                    if (engineRecords.Length == 0 && engineErrors.Length == 0)
                        return new List<string> { LogMessages.NOREADINGSFOUND };

                    return new List<string>();
                }
            }
        }

        private void InsertAndUpdateFile(string file, InputFileInsertDTO dto, List<string> errors)
        {
            using (var context = _contextFactory.Create())
            {
                if (!errors.Any())
                {
                    _inputFileRepository.Insert(dto, context);
                    SendToBackup(file);
                }

                var inputFileId = context.Query<long>
                    ($@"SELECT INPUTFILEID FROM INPUTFILE 
                        WHERE BYTES = @BYTES AND NAME = @NAME AND 
                        STATUS = @STATUS AND EXTENSION = @EXTENSION", dto).First();

                var updateDTOError = new InputFileUpdateDTO
                                    (inputFileId, Status.Error, errors);

                _inputFileRepository.Insert(dto, context);
                _inputFileRepository.Update(updateDTOError, context);
            }
        }

        private void SendToBackup(string filePath)
        {
            string backupPath = Path.Combine(filePath, "BACKUP");
            File.Move(filePath, backupPath);
        }
    }
}