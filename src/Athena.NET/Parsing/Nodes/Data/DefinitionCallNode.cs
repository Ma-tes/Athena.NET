using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes.Data;

internal sealed class DefinitionCallNode : DataNode<ReadOnlyMemory<INode>>
{
    public IdentifierNode DefinitionIdentifier { get; }

    public DefinitionCallNode(IdentifierNode definitionIdentifier, ReadOnlyMemory<INode> argumentNodes)
        : base(Lexing.TokenIndentificator.DefinitionCall, argumentNodes)
    {
        DefinitionIdentifier = definitionIdentifier;
    }
}
