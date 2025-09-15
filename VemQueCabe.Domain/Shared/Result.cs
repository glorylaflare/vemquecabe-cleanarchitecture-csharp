namespace VemQueCabe.Domain.Shared;

/// <summary>
/// Represents the result of an operation, indicating success or failure.
/// </summary>
public record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// Represents the result of an operation with a value, indicating success or failure.
/// </summary>
/// <typeparam name="T">The type of the value associated with the result.</typeparam>
public record Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null)
    {
        Value = value;
    }

    private Result(Error error) : base(false, error) { }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}
