namespace Athena.NET.ExceptionResult.Interfaces;

public interface IResultProvider<T>
{
    public IResult<T> ValueResult { get; }
    public string? Message { get; }

    public void LogMessage();
}
