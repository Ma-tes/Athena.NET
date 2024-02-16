using Athena.NET.ExceptionResult.Interfaces;

namespace Athena.NET.ExceptionResult;

public sealed class ErrorResult<T> : IResultProvider<T>
{
    public Type ProviderHolderType { get; } = typeof(ErrorResult<T>);
    public int ExceptionIndex { get; private set; }

    public IResult<T> ValueResult { get; }
    public string? Message { get; }

    public ErrorResult(IResult<T> valueResult, string message)
    {
        ValueResult = valueResult; 
        Message = message;
    }

    public static ErrorResult<T> Create(string message, IResult<T> valueResult = null!) =>
        new ErrorResult<T>(valueResult, message);
    public static ErrorResult<T> CreateNullResult(string message, int exceptioIndex)
    {
        var currentErrorResult = ErrorResult<T>.Create(message);
        currentErrorResult.ExceptionIndex = exceptioIndex;
        return currentErrorResult;
    }

    public void LogMessage() =>
        Console.WriteLine($"[{ValueResult.PositionIndex}]: {Message}");
}
