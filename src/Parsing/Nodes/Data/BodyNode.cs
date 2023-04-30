using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes.Data;

public sealed class BodyNode : DataNode<ReadOnlyMemory<INode>>
{
    public BodyNode(ReadOnlyMemory<INode> bodyNodes) : base(Lexing.TokenIndentificator.Unknown, bodyNodes)
    {
    }
}
