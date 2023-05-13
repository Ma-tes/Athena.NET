﻿using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Statements;
using Athena.NET.Parsing.Nodes.Statements.Body;

namespace Athena.NET.Compilation.Instructions.Definition;

internal static class DefinitionHelper 
{
    public static ReadOnlySpan<int> CreateDefinitionsCallOrder(ReadOnlySpan<INode> definitionStatements)
    {
    }

    private static Span<int> GetDefinitionCallOrder(ReadOnlySpan<DefinitionStatement> definitionStatements,
        ReadOnlySpan<INode> definitionNodes)
    {
        using var definitionCallOrderList = new NativeMemoryList<int>();
        int definitionNodesLength = definitionNodes.Length;
        for (int i = 0; i < definitionNodesLength; i++)
        {
            if (definitionNodes[i] is CallStatement currentCallStatement)
            {
                InstanceNode callInstanceNode = (InstanceNode)currentCallStatement.ChildNodes.LeftNode;
                if (!TryGetDefinitionStatementInstance(out DefinitionStatement currentDefinition,
                    definitionStatements, callInstanceNode))
                    throw new Exception($"Definition {callInstanceNode.NodeData.ToArray()} wasn't found");

                BodyNode definitionBodyNodes = (BodyNode)currentDefinition.ChildNodes.RightNode;
                Span<int> currentCallOrder = GetDefinitionCallOrder(definitionStatements, definitionBodyNodes.NodeData.Span);
                definitionCallOrderList.AddRange(currentCallOrder);
            }
        }
        return definitionCallOrderList.Span;
    }

    private static bool TryGetDefinitionStatements(out ReadOnlyMemory<DefinitionStatement> returnStatements,
        ReadOnlySpan<INode> nodes)
    {
        Memory<DefinitionStatement> definitionStatements = new DefinitionStatement[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] is not DefinitionStatement currentStatement)
                return NullableHelper.NullableOutValue(out returnStatements);
            definitionStatements.Span[i] = currentStatement;
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
        return NullableHelper.NullableOutValue(out returnStatement);
    }
}
