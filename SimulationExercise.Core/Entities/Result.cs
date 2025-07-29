namespace SimulationExercise.Core.Entities
{
    public class Result<T>
    {
        private readonly T? _value;

        private Result(T? value, IList<string>? errors, bool success)
        {
            _value = value;
            Errors = errors;
            Success = success;
        }

        public T? Value
        {
            get
            {
                if (!Success)
                {
                    throw new InvalidOperationException
                    ("Cannot access Value if Success is false");
                }

                return _value;
            }
        }

        public IList<string>? Errors { get; }

        public bool Success { get; }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(value, null, success: true);
        }

        public static Result<T> Ko(IList<string> errors)
        {
            if (errors == null) throw new ArgumentNullException
                                               (nameof(errors));

            if (errors.Count == 0) throw new InvalidOperationException
                              ("Empty list of errors for a ko result");

            return new Result<T>(default, errors, success: false);
        }
    }
}
