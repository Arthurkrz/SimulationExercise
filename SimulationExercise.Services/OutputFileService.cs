using FileHelpers;
using Microsoft.Extensions.Logging;
using SimulationExercise.Core.Common;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using System.Text;

namespace SimulationExercise.Services
{
    public class OutputFileService : IOutputFileService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IConsistentReadingRepository _consistentReadingRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IConsistentReadingExportDTOFactory _consistentReadingExportDTOFactory;
        private readonly ILogger<OutputFileService> _logger;

        public OutputFileService(IContextFactory contextFactory,
                                 IConsistentReadingRepository consistentReadingRepository,
                                 IOutputFileRepository outputFileRepository,
                                 IConsistentReadingExportDTOFactory consistentReadingExportDTOFactory,
                                 ILogger<OutputFileService> logger)
        {
            _contextFactory = contextFactory;
            _consistentReadingRepository = consistentReadingRepository;
            _outputFileRepository = outputFileRepository;
            _consistentReadingExportDTOFactory = consistentReadingExportDTOFactory;
            _logger = logger;
        }

        public void ProcessConsistentReadings()
        {
            var engine = new FileHelperEngine<OutputFileService>();
            IList<ConsistentReadingGetDTO> crGetDTOs = null;
            using (IContext searchContext = _contextFactory.Create())
            {
                crGetDTOs = _consistentReadingRepository.GetByStatus(
                    Status.New, searchContext);
            }

            if (crGetDTOs.Count == 0)
            {
                _logger.LogError(LogMessages.NONEWOBJECTSFOUND, "Consistent Reading");
                return;
            }

            var records = _consistentReadingExportDTOFactory
                .CreateExportDTOList(crGetDTOs);

            var exportFile = CreateExportFile(records);

            var insertDTO = new OutputFileInsertDTO(
                exportFile.Name, exportFile.Bytes, 
                exportFile.Extension, Status.New);

            using (IContext context = _contextFactory.Create())
            { InsertOutputFile(crGetDTOs, insertDTO, context); }
        }

        private OutputFileInsertDTO CreateExportFile(IList<ConsistentReadingExportDTO> records)
        {
            var engine = new FileHelperEngine<ConsistentReadingExportDTO>();

            string fileHeader = string.Join(",", typeof(ConsistentReading)
                                      .GetFields().Select(f => f.Name));

            var csvFile = fileHeader + Environment.NewLine + engine.WriteString(records);
            var csvBytes = Encoding.UTF8.GetBytes(csvFile);
            var fileName = $"Reading{SystemTime.Now():dd_MM_yyyy}";
            var fileExtension = ".csv";

            return new OutputFileInsertDTO(fileName, csvBytes, fileExtension, Status.Success);
        }

        private void InsertOutputFile(IList<ConsistentReadingGetDTO> crGetDTOs, OutputFileInsertDTO insertDTO, IContext context)
        {
            try { _outputFileRepository.Insert(insertDTO, context); }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.ERRORWHENINSERTINGFILE, 
                                 insertDTO.Name, ex.Message);
                return;
            }

            foreach (var crGetDTO in crGetDTOs)
                UpdateConsistentReading(crGetDTO, context);
        }

        private void UpdateConsistentReading(ConsistentReadingGetDTO getDTO, IContext context)
        {
            var crUpdateDTO = new ConsistentReadingUpdateDTO
                (getDTO.ConsistentReadingId, Status.New, 
                 new List<string>());

            try 
            { 
                _consistentReadingRepository.Update(
                    crUpdateDTO, context); 
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.ERRORWHENUPDATINGOBJECT, 
                                 "Consistent Reading", ex.Message);
            }
        }
    }
}