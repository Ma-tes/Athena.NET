using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser.Nodes.DataNodes
{
    internal class DataNode<T> : INode
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
    }
}
