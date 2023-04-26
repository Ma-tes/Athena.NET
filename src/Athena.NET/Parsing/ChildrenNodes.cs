using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing;

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
