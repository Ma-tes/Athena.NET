using Athena.NET.Lexing;
using Athena.NET.Parsing.Nodes.Data;

namespace Athena.NET.Parsing.Nodes;

internal sealed class IdentifierNode : DataNode<ReadOnlyMemory<char>>
{
    private static readonly TokenIndentificator identifierToken =
        TokenIndentificator.Identifier;

    public IdentifierNode(ReadOnlyMemory<char> data) : base(identifierToken, data)
    {
    }
}
