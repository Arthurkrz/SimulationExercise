using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.DTOs.DatabaseDTOs;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IAverageProvinceDataRepository
    {
        void Insert(AverageProvinceDataInsertDTO dto, IContext context);
        void Update(AverageProvinceDataUpdateDTO dto, IContext context);
        IList<AverageProvinceDataGetDTO> GetByIsExported(bool isExported, IContext context);
    }
}
