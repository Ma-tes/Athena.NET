using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Nodes;

namespace Athena.NET.Parsing.Interfaces;

public interface INode
{
    public TokenIndentificator NodeToken { get; }
    public ChildrenNodes ChildNodes { get; }

    public NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex);
}
