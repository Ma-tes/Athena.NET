using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Data;

//Maybe I should separete them into parent instance node
//that contains children node identifier
internal sealed class InstanceNode : DataNode<ReadOnlyMemory<char>>
{
    public InstanceNode(TokenIndentificator typeToken, ReadOnlyMemory<char> identifierName) : base(typeToken, identifierName)
    {
    }
}
