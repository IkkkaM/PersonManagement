namespace PersonDirectory.Application.Commons;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> ValidationErrors { get; private set; } = new();

    private Result(bool isSuccess, T? data, string? errorMessage, List<string>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors ?? new List<string>();
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    public static Result<T> ValidationFailure(List<string> validationErrors) => new(false, default, "Validation failed", validationErrors);
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> ValidationErrors { get; private set; } = new();

    private Result(bool isSuccess, string? errorMessage, List<string>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors ?? new List<string>();
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
    public static Result ValidationFailure(List<string> validationErrors) => new(false, "Validation failed", validationErrors);
}