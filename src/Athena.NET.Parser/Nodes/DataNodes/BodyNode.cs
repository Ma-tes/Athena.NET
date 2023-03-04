using Athena.NET.Parser.Interfaces;

namespace Athena.NET.Parser.Nodes.DataNodes
{
    internal sealed class BodyNode : DataNode<ReadOnlyMemory<INode>>
    {
        public BodyNode(ReadOnlyMemory<INode> bodyNodes) : base(Lexer.TokenIndentificator.Unknown, bodyNodes)
        {
        }
    }
}
