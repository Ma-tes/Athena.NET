using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser.Nodes.DataNodes
{
    //I'm still not sure about
    //this implementation, maybe I should
    //create a different implementation
    //of this data holding node
    public class DataNode<T> : INode
    {
        public TokenIndentificator NodeToken { get; }
        public ChildrenNodes ChildNodes { get; set; } =
            ChildrenNodes.BlankNodes;

        public T NodeData { get; }

        public DataNode(TokenIndentificator token, T data) 
        {
            NodeToken = token;
            NodeData = data;
        }

        public NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex) =>
            new SuccessulNodeResult<INode>(this);
    }
}
