using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser.Nodes.DataNodes
{
    //I'm still not sure about
    //this implementation, maybe I should
    //create a different implementation
    //of this data holding node
    internal class DataNode<T> : INode
    {
        public TokenIndentificator NodeToken { get; }
        public ChildrenNodes ChildNodes { get; set; } =
            ChildrenNodes.BlankNodes;
        public int ChildNodesCount { get; } = 1;

        public T NodeData { get; }

        public DataNode(TokenIndentificator token, T data) 
        {
            NodeToken = token;
            NodeData = data;
        }

        public ChildrenNodes SepareteNodes(ReadOnlyMemory<Token> tokens, int nodeIndex) =>
            ChildrenNodes.BlankNodes;
    }
}
