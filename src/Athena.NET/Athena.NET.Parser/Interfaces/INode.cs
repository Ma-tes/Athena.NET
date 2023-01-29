using Athena.NET.Athena.NET.Lexer.Structures;

namespace Athena.NET.Athena.NET.Parser.Interfaces
{
    internal interface INode
    {
        public TokenIndentificator NodeToken { get; }
        public ChildrenNodes ChildNodes { get; }
        public int ChildNodesCount { get; }

        public ChildrenNodes SepareteNodes(ReadOnlyMemory<Token> tokens, int nodeIndex);
    }
}
