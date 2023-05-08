using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Statements.Body;

namespace Athena.NET.Compilation.Instructions.Definition;

internal static class DefinitionHelper 
{
    public static ReadOnlySpan<int> CreateDefinitionsCallOrder(ReadOnlySpan<DefinitionStatement> definitionStatements) 
    {
    }

    private static Span<int> GetDefinitionCallOrder(ReadOnlySpan<DefinitionStatement> definitionStatements,
        ReadOnlySpan<INode> definitionNodes)
    {
    }

    private static bool TryGetDefinitionStatements(out ReadOnlySpan<DefinitionStatement> returnStatements,
        ReadOnlySpan<INode> nodes)
    {
        Span<DefinitionStatement> definitionStatements = new DefinitionStatement[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] is not DefinitionStatement currentStatement)
            {
                returnStatements = null;
                return false;
            }
            definitionStatements[i] = currentStatement;
        }
        returnStatements = definitionStatements;
        return true;
    }
 
    private static bool TryGetDefinitionStatementInstance(out DefinitionStatement returnStatement, ReadOnlySpan<DefinitionStatement> definitionStatements,
        InstanceNode instanceNode)
    {
        uint instanceIdentificator = MemoryData.CalculateIdentifierId(instanceNode.NodeData);
        int definitionsLength = definitionStatements.Length;
        for (int i = 0; i < definitionsLength; i++)
        {
            var currentDefinitionNode = (DefinitionNode)definitionStatements[i].ChildNodes.LeftNode;
            uint currentDefinitionIdentifactor = MemoryData.CalculateIdentifierId(currentDefinitionNode.DefinitionIdentifier.NodeData);
            if (currentDefinitionIdentifactor == instanceIdentificator) 
            {
                returnStatement = definitionStatements[i];
                return true;
            }
        }
        returnStatement = null!;
        return false;
    }
}
