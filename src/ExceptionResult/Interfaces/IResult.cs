namespace Athena.NET.ExceptionResult.Interfaces;

internal enum ResultType 
{
    Lexing,
    Parsing,
    Interpreting
}

internal interface IResult<T>
{
    public T? Result { get; }
    public ResultType Type { get; }
    public int PositionIndex { get; }
}
