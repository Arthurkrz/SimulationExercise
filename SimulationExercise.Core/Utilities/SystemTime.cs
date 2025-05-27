namespace SimulationExercise.Core.Utilities
{
    public class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}
