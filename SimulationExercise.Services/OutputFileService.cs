using FileHelpers;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.Contracts.Services;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Entities;
using SimulationExercise.Core.Utilities;
using System.Text;

namespace SimulationExercise.Services
{
    public class OutputFileService : IOutputFileService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IOutputFileRepository _outputFileRepository;

        public OutputFileService(IContextFactory contextFactory, IOutputFileRepository outputFileRepository)
        {
            _contextFactory = contextFactory;
            _outputFileRepository = outputFileRepository;
        }

        public Result<OutputFileInsertDTO> CreateOutputFiles<T>(IList<T> objs) where T : class
        {
            Type type = typeof(T);
            var engine = new FileHelperEngine<T>();

            string fileHeader = string.Join(",", typeof(T).GetProperties().Select(p => p.Name));

            var csvFile = fileHeader + Environment.NewLine + engine.WriteString(objs);
            var csvBytes = Encoding.UTF8.GetBytes(csvFile);
            var fileName = $"{typeof(T).Name}{SystemTime.Now():dd_MM_yyyy}";
            var fileExtension = ".csv";

            var insertDTO = new OutputFileInsertDTO(fileName, csvBytes, fileExtension, typeof(T), false);

            using (IContext insertContext = _contextFactory.Create())
            {
                _outputFileRepository.Insert(insertDTO, insertContext);
                insertContext.Commit();
            }

            return Result<OutputFileInsertDTO>.Ok(insertDTO);
        }

        public void Export<T>(OutputFileGetDTO obj, Stream outputStream) where T : class
        {
            var engine = new FileHelperEngine<T>();
            using (var sr = new StreamReader(outputStream))
            {
                var records = engine.ReadStream(sr);

                using (var sw = new StreamWriter(outputStream, leaveOpen: true))
                    engine.WriteStream(sw, records);
            }
        }
    }
}