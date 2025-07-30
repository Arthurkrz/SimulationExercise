using SimulationExercise.Core.DatabaseDTOs;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IOutputFileRepository
    {
        void Insert(OutputFileInsertDTO dto, IContext context);
        void Update(OutputFileUpdateDTO dto, IContext context);
        IList<OutputFileGetDTO> GetByStatus(Status status, IContext context);
        IList<OutputFileGetDTO> GetByIsAverageProvinceDataExported(bool isExported, IContext context);
    }
}
