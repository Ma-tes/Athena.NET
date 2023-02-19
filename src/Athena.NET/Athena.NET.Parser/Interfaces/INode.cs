using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Nodes;

namespace Athena.NET.Athena.NET.Parser.Interfaces
{
    public interface INode
    {
        public TokenIndentificator NodeToken { get; }
        public ChildrenNodes ChildNodes { get; }

        public NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex);
    }
}
