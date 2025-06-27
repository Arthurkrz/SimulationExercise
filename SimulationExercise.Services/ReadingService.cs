using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileRepository _inputFileRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadingImportService _readingImportService;

        public ReadingService(IContextFactory contextFactory, IInputFileRepository inputFileRepository, IReadingImportService readingImportService, IReadingRepository readingRepository)
        {
            _contextFactory = contextFactory;
            _inputFileRepository = inputFileRepository;
            _readingRepository = readingRepository;
            _readingImportService = readingImportService;
        }

        public void ProcessInputFiles()
        {
            using (IContext context = _contextFactory.Create())
            {
                var inputFiles = _inputFileRepository.GetByStatus
                                            (Status.New, context);

                foreach (var inputFile in inputFiles)
                {
                    List<ReadingInsertDTO> inserts = new List<ReadingInsertDTO>();
                    List<ReadingUpdateDTO> updates = new List<ReadingUpdateDTO>();
                    List<string> errors = new List<string>();

                    using (var stream = new MemoryStream(inputFile.Bytes))
                    {
                        ImportResult importResult = _readingImportService.Import(stream);
                 
                        if (importResult.Success)
                        {
                            var readingInsertDTO = new ReadingInsertDTO(inputFile.InputFileId, inputFile.Bytes, Status.New);
                            inserts.Add(readingInsertDTO);
                        }

                        foreach (var error in importResult.Errors)
                        {
                            var readingInsertErrorDTO = new ReadingInsertDTO(inputFile.InputFileId, inputFile.Bytes, Status.Error);
                            
                        }

                        inserts.Add(readingInsertErrorDTO);
                        errors.Add(impor)
                    }

                    var filteredInserts = FilterObjects(inserts);
                }
            }
        }

        private void InsertAndUpdateReading(List<ReadingInsertDTO> inserts, List<ReadingUpdateDTO> updates)
        {

        }

        private int GetReadingId(ReadingInsertDTO dto)
        {

        }

        private List<ReadingInsertDTO> FilterObjects(List<ReadingInsertDTO> inserts)
        {
            using (IContext context = _contextFactory.Create())
            {
                var newReadings = _readingRepository.GetByStatus(Status.New, context);
                var errorReadings = _readingRepository.GetByStatus(Status.Error, context);

                foreach (var insert in inserts)
                {
                    if (newReadings.Any(x => x.InputFileId == insert.InputFileId))
                        inserts.Remove(insert);

                    if (errorReadings.Any(x => x.InputFileId == insert.InputFileId))
                        inserts.Remove(insert);
                }

                return inserts;
            }
        }
    }
}