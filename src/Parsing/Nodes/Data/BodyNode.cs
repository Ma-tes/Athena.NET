using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes.Data;

internal sealed class BodyNode : DataNode<ReadOnlyMemory<INode>>
{
    public BodyNode(ReadOnlyMemory<INode> bodyNodes) : base(Lexing.TokenIndentificator.Unknown, bodyNodes)
    {
    }
}
