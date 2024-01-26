using Athena.NET.ExceptionResult.Interfaces;

namespace Athena.NET.ExceptionResult;

public sealed class SuccessfulResult<T> : IResultProvider<T>
{
    public IResult<T> ValueResult { get; }
    public string? Message { get; internal set; }

    public SuccessfulResult(IResult<T> valueResult)
    {
        ValueResult = valueResult;
    }

    public static SuccessfulResult<T> Create(IResult<T> valueResult) =>
        new SuccessfulResult<T>(valueResult);
    public static SuccessfulResult<T> Create<TResult>(T resultValue, int positionIndex)
        where TResult : IResult<T>
    {
        ArgumentNullException.ThrowIfNull(resultValue, "Invalid create invoke, with nullable result value");
        return new SuccessfulResult<T>(
           (TResult)Activator.CreateInstance(typeof(TResult), [resultValue!, positionIndex])!
        );
    }

    public void LogMessage() =>
        Console.WriteLine(Message is null ?
            "Result was successfully created" : Message);
}
