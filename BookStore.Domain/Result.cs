namespace BookStore.Domain;

public class Result<T>
{
    public bool Success { get; private set; }
    public T Value { get; private set; }
    public Dictionary<string, string> Errors { get; private set; } = new();

    public static Result<T> Succeeded => new Result<T>().SetSuccess(true);
    public static Result<T> Failed => new Result<T>().SetSuccess(false);

    public Result<T> SetValue(T value)
    {
        Value = value;
        return this;
    }

    public Result<T> AddError(string key, string message)
    {
        Success = false;
        Errors.TryAdd(key, message);
        return this;
    }

    public Result<T> AddErrors(Dictionary<string, string> errors)
    {
        Success = false;
        Errors = errors;
        return this;
    }

    public Result<T> SetSuccess(bool succeeded)
    {
        Success = succeeded;
        return this;
    }
}

public class Result
{
    public bool Success { get; private set; }
    public Dictionary<string, string> Errors { get; } = new();

    public static Result Succeeded => new Result().SetSuccess(true);
    public static Result Failed => new Result().SetSuccess(false);

    public Result AddError(string key, string message)
    {
        Success = false;
        Errors.TryAdd(key, message);
        return this;
    }

    public Result SetSuccess(bool succeeded)
    {
        Success = succeeded;
        return this;
    }
}