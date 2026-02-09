namespace AgendaPlus.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, string error, Exception? exception = null)
    {
        switch (isSuccess)
        {
            case true when error != string.Empty:
                throw new InvalidOperationException("Success result cannot have an error message.");
            case false when error == string.Empty:
                throw new InvalidOperationException("Failure result must have an error message.");
            default:
                IsSuccess = isSuccess;
                Error = error;
                Exception = exception;
                break;
        }
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public Exception? Exception { get; }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Failure(string error, Exception? exception = null)
    {
        return new Result(false, error, exception);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Failure<T>(string error, Exception? exception = null)
    {
        return new Result<T>(default!, false, error, exception);
    }
}

public class Result<T> : Result
{
    protected internal Result(T value, bool isSuccess, string error, Exception? exception = null) : base(isSuccess,
        error, exception)
    {
        Value = value;
    }

    public T Value { get; }
}