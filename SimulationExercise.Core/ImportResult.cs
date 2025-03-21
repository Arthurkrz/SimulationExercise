using SimulationExercise.Core.Contracts;

namespace SimulationExercise.Core
{
    public class ImportResult
    {
        public ImportResult(IList<Reading> readings, IList<string> errors)
		{
			Readings = readings ?? throw new ArgumentNullException(nameof(readings));
			Errors = errors ?? throw new ArgumentNullException(nameof(errors));
			Success = errors.Count == 0;
		}

		public IList<Reading> Readings { get; set; }
		public IList<string> Errors { get; set; }
        public bool Success { get; set; }
    }
}
