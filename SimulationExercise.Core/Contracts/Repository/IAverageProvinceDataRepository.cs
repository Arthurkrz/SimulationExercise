using SimulationExercise.Core.DatabaseDTOs;
using SimulationExercise.Core.DTOS;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IAverageProvinceDataRepository
    {
        void Insert(AverageProvinceDataInsertDTO dto, IContext context);
        void Update(AverageProvinceDataUpdateDTO dto, IContext context);
        IList<AverageProvinceDataGetDTO> GetByIsExported(bool isExported, IContext context);
    }
}
