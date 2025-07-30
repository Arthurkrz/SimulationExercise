using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.Contracts.Repository
{
    public interface IAverageProvinceDataRepository
    {
        void Insert(AverageProvinceDataInsertDTO dto, IContext context);
        void Update(AverageProvinceDataUpdateDTO dto, IContext context);
        IList<AverageProvinceDataGetDTO> GetByStatus(Status status, IContext context);
    }
}
