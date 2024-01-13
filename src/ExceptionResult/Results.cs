using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.ExceptionResult;

public sealed record class LexingResult(Token Result, int PositionIndex) : IResult<Token>
{
    public ResultType Type => ResultType.Lexing;
}

public sealed record class ParsingResult(INode? Result, int PositionIndex) : IResult<INode>
{
    public ResultType Type => ResultType.Parsing;
}

public sealed record class CompilationResult(INode? Result, int PositionIndex) : IResult<INode>
{
    public ResultType Type => ResultType.Parsing;
}
