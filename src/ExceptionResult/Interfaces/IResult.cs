﻿namespace Athena.NET.ExceptionResult.Interfaces;

public enum ResultType 
{
    Lexing,
    Parsing,
    Compilation
}

public interface IResult<T>
{
    public T? Result { get; }
    public ResultType Type { get; }
    public int PositionIndex { get; }
}
