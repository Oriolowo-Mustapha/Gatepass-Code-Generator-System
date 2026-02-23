namespace Application.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException() : base("One or more validation failures occurred.")
    {
        Errors = [];
    }

    public ValidationException(IEnumerable<string> errors) : this()
    {
        Errors = errors.ToList();
    }
}
