namespace Athena.NET.ExceptionResult.Interfaces;

internal interface IResultProvider<T>
{
    public IResult<T> ValueResult { get; }
    public string? Message { get; }

    public void LogMessage();
}
