using Athena.NET.ExceptionResult.Interfaces;

namespace Athena.NET.ExceptionResult;

public sealed class ErrorResult<T> : IResultProvider<T>
{
    public IResult<T> ValueResult { get; }
    public string? Message { get; }

    public ErrorResult(IResult<T> valueResult, string message)
    {
        ValueResult = valueResult; 
        Message = message;
    }

    public static ErrorResult<T> Create(IResult<T> valueResult = null!, string message) =>
        new ErrorResult<T>(valueResult, message);

    public void LogMessage() =>
        Console.WriteLine($"[{ValueResult.PositionIndex}]: {Message}");
}
