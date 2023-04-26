using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Data;

//TODO: Consider separating this into "parent instance"
//with children identifiers
internal sealed class InstanceNode : DataNode<ReadOnlyMemory<char>>
{
    public InstanceNode(TokenIndentificator typeToken, ReadOnlyMemory<char> identifierName) : base(typeToken, identifierName)
    {
    }
}
