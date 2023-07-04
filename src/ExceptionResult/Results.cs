using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.ExceptionResult;

internal sealed record class ParsingResult(INode? Result, int PositionIndex) : IResult<INode>
{
    public ResultType Type => ResultType.Parsing;
}

internal sealed record class LexingResult(Token Result, int PositionIndex) : IResult<Token>
{
    public ResultType Type => ResultType.Lexing;
}

internal sealed record class InterpretationResult(INode? Result, int PositionIndex) : IResult<INode>
{
    public ResultType Type => ResultType.Lexing;
}
