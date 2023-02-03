using Athena.NET.Athena.NET.Lexer;

namespace Athena.NET.Athena.NET.Parser.Interfaces
{
    public interface INode
    {
        public TokenIndentificator NodeToken { get; }
        public ChildrenNodes ChildNodes { get; }
        public int ChildNodesCount { get; }
    }
}
