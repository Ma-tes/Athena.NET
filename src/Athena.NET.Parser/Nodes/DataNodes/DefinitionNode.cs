using Athena.NET.Lexer;

namespace Athena.NET.Parser.Nodes.DataNodes
{
    internal sealed class DefinitionNode : DataNode<ReadOnlyMemory<InstanceNode>>
    {
        public DefinitionNode(TokenIndentificator definitionToken, ReadOnlyMemory<InstanceNode> argumentInstances) 
            : base(definitionToken, argumentInstances)
        {
        } 
    }
}
