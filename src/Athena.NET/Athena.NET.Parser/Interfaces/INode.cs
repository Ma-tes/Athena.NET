using Athena.NET.Athena.NET.Lexer.Structures;

namespace Athena.NET.Athena.NET.Parser.Interfaces
{
    internal interface INode
    {
        public abstract Token NodeToken { get; }
        public ChildrenNodes ChildNodes { get; }
    }
}
