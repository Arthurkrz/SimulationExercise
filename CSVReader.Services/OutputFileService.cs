using FileHelpers;
using Microsoft.Extensions.Logging;
using CSVReader.Core.Common;
using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.Contracts.Services;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Entities;
using CSVReader.Core.Utilities;
using System.Text;

namespace CSVReader.Services
{
    public class OutputFileService : IOutputFileService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly ILogger<OutputFileService> _logger;

        public OutputFileService(IContextFactory contextFactory, IOutputFileRepository outputFileRepository, ILogger<OutputFileService> logger)
        {
            _contextFactory = contextFactory;
            _outputFileRepository = outputFileRepository;
            _logger = logger;
        }

        public async Task<Result<OutputFileInsertDTO>> CreateOutputFilesAsync<T>(IList<T> objs) where T : class
        {
            Type type = typeof(T);
            var engine = new FileHelperEngine<T>();

            string fileHeader = string.Join(";", typeof(T).GetProperties().Select(p => p.Name));

            var typeName = type.Name.Replace("ExportDTO", "");
            var csvFile = fileHeader + Environment.NewLine + engine.WriteString(objs);
            var csvBytes = Encoding.UTF8.GetBytes(csvFile);
            var fileName = $"{typeName}{SystemTime.Now():dd_MM_yyyy}";
            var fileExtension = ".csv";

            var insertDTO = new OutputFileInsertDTO(fileName, csvBytes, fileExtension, typeName, false);

            using (IContext insertContext = _contextFactory.Create())
            {
                try
                {
                    await _outputFileRepository.InsertAsync(insertDTO, insertContext);
                    insertContext.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogMessages.UNEXPECTEDEXCEPTION, ex.Message);
                }
                finally
                {
                    insertContext.Dispose();
                }
            }

            return Result<OutputFileInsertDTO>.Ok(insertDTO);
        }
    }
}