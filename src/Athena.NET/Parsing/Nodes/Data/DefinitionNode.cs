using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;

namespace Athena.NET.Parsing.Nodes.Data;

internal sealed class DefinitionNode : DataNode<ReadOnlyMemory<InstanceNode>>
{
    public IdentifierNode DefinitionIdentifier { get; }

    public DefinitionNode(TokenIndentificator definitionToken, Token identificator, ReadOnlyMemory<InstanceNode> argumentInstances)
        : base(definitionToken, argumentInstances)
    {
        DefinitionIdentifier = new IdentifierNode(identificator.Data);
    }
}
