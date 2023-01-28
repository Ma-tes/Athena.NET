using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser
{
    internal readonly struct ChildrenNodes
    {
        public INode LeftNode { get; }
        public INode RightNode { get; }

        public ChildrenNodes(INode leftNode, INode rightNode) 
        {
            LeftNode = leftNode;
            RightNode = rightNode;
        }
    }
}
