using SimulationExercise.Core.Enum;

namespace SimulationExercise.Core.DTOS
{
    public class AverageProvinceDataUpdateDTO
    {
        public AverageProvinceDataUpdateDTO(long averageProvinceDataId, Status status, IList<string> messages)
        {
            if (status == Status.Error && (messages == null || !messages.Any()))
                throw new ArgumentNullException("Update as error without error message list is not allowed.");

            else if (status == Status.Success && messages != null)
                throw new ArgumentException("Update as success with error message list is not allowed.");

            AverageProvinceDataId = averageProvinceDataId;
            Status = status;
            Messages = messages ?? new List<string>();
        }

        public long AverageProvinceDataId { get; }
        public Status Status { get; }
        public IList<string> Messages { get; }
    }
}
