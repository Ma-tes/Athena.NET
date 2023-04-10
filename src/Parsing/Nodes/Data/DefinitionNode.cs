using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Data;

internal sealed class DefinitionNode : DataNode<ReadOnlyMemory<InstanceNode>>
{
    public DefinitionNode(TokenIndentificator definitionToken, ReadOnlyMemory<InstanceNode> argumentInstances)
        : base(definitionToken, argumentInstances)
    {
    }
}
