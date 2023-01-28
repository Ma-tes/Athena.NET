using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser.Nodes
{
    internal abstract class StatementNode<T, TReturn> : INode where T : StatementNode<T, TReturn>
    {
        public abstract Token NodeToken { get; }
        public ChildrenNodes ChildNodes { get; }

        public StatementNode(ReadOnlyMemory<Token> tokens) 
        {

        }

        public abstract TReturn Evaluate();
    }
}
