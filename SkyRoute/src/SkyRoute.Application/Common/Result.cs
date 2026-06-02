namespace SkyRoute.Application.Common;

public sealed class Result<T>
{
    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Errors = [];
    }

    private Result(IEnumerable<string> errors)
    {
        Errors = errors.ToList();
        IsSuccess = false;
    }

    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors { get; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(params string[] errors) => new(errors.AsEnumerable());
    public static Result<T> Failure(IEnumerable<string> errors) => new(errors);
}
