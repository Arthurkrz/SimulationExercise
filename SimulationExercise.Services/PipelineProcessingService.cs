using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;

namespace SimulationExercise.Services
{
    public class PipelineProcessingService : IPipelineProcessingService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IInputFileService _inputFileService;
        private readonly IReadingService _readingService;
        private readonly IConsistentReadingService _consistentReadingService;
        private readonly IOutputFileService _outputFileService;

        public PipelineProcessingService(IContextFactory contextFactory,
                                         IInputFileService inputFileService,
                                         IReadingService readingService,
                                         IConsistentReadingService consistentReadingService,
                                         IOutputFileService outputFileService)
        {
            _contextFactory = contextFactory;
            _inputFileService = inputFileService;
            _readingService = readingService;
            _consistentReadingService = consistentReadingService;
            _outputFileService = outputFileService;
        }

        public void Initialize(string inDirectoryPath) =>
            _inputFileService.ProcessFiles(inDirectoryPath);

        public void ProcessInputFiles() => 
            _readingService.ProcessInputFiles();

        public void ProcessReadings() =>
            _consistentReadingService.ProcessReadings();

        public void ProcessConsistentReadings() =>
            _outputFileService.ProcessConsistentReadings();
    }
}
