using Athena.NET.ExceptionResult.Interfaces;

namespace Athena.NET.ExceptionResult;

internal sealed class SuccessfulResult<T> : IResultProvider<T>
{
    public IResult<T> ValueResult { get; }
    public string? Message { get; internal set; }

    public SuccessfulResult(IResult<T> valueResult) 
    {
        ValueResult = valueResult;
    }

    public static SuccessfulResult<T> Create(IResult<T> valueResult) =>
        new SuccessfulResult<T>(valueResult);

    public void LogMessage() =>
        Console.WriteLine(Message is null ?
            "Result was successfully created" : Message);
}
