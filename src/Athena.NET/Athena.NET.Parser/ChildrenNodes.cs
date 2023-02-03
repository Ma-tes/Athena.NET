using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser
{
    public readonly struct ChildrenNodes
    {
        public INode LeftNode { get; }
        public INode RightNode { get; }

        public static ChildrenNodes BlankNodes { get; } =
            new(null!, null!);

        public ChildrenNodes(INode leftNode, INode rightNode)
        {
            LeftNode = leftNode;
            RightNode = rightNode;
        }
    }
}
