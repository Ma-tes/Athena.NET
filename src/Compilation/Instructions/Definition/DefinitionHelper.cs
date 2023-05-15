using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Statements;
using Athena.NET.Parsing.Nodes.Statements.Body;

namespace Athena.NET.Compilation.Instructions.Definition;

internal static class DefinitionHelper 
{
    public static ReadOnlySpan<int> CreateDefinitionsCallOrder(ReadOnlySpan<INode> nodes)
    {
        Span<DefinitionStatement> definitionStatements = GetDefinitionStatements(nodes);
        if(TryGetDefinitionStatementInstance(out DefinitionStatement mainDefinitionStatement, definitionStatements,
            InstructionWriter.MainDefinitionIdentificator) == -1)
            throw new Exception("Main definition wasn't found");
        return CreateRelativeCallOrder(mainDefinitionStatement, definitionStatements, definitionStatements, 0);
    }

    private static Span<int> CreateRelativeCallOrder(DefinitionStatement definitionStatement, 
        Span<DefinitionStatement> definitionStatements, ReadOnlySpan<DefinitionStatement> originalStatements, int relativeIndex)
    {
        BodyNode definitionBodyNodes = (BodyNode)definitionStatement.ChildNodes.RightNode;
        Span<DefinitionStatement> currentDefinitionStatements = ReallocateOnSpan(definitionStatements, relativeIndex);
        return GetDefinitionCallOrder(currentDefinitionStatements, originalStatements, definitionBodyNodes.NodeData.Span);
    }

    private static Span<int> GetDefinitionCallOrder(Span<DefinitionStatement> definitionStatements, ReadOnlySpan<DefinitionStatement> originalStatements,
        ReadOnlySpan<INode> definitionNodes)
    {
        var definitionCallOrderList = new NativeMemoryList<int>();
        int definitionNodesLength = definitionNodes.Length;
        for (int i = 0; i < definitionNodesLength; i++)
        {
            if (definitionNodes[i] is CallStatement currentCallStatement)
            {
                IdentifierNode callIdentifierNode = (IdentifierNode)currentCallStatement.ChildNodes.LeftNode;
                uint identifierId = MemoryData.CalculateIdentifierId(callIdentifierNode.NodeData);

                int currentStatementIndex = TryGetDefinitionStatementInstance(out DefinitionStatement currentDefinition,
                    definitionStatements, identifierId);
                int originalStatementIndex = TryGetDefinitionStatementInstance(out _,
                    originalStatements, identifierId);
                if (currentStatementIndex != -1)
                {
                    Span<int> currentRelativeCallOrder = CreateRelativeCallOrder(currentDefinition, definitionStatements, originalStatements, currentStatementIndex);
                    definitionCallOrderList.Add(originalStatementIndex);
                    definitionCallOrderList.AddRange(currentRelativeCallOrder);
                }
            }
        }
        return definitionCallOrderList.Span;
    }

    private static Span<DefinitionStatement> GetDefinitionStatements(ReadOnlySpan<INode> nodes)
    {
        Span<DefinitionStatement> definitionStatements = new DefinitionStatement[nodes.Length];
        int definitionCount = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] is DefinitionStatement currentStatement) 
            {
                definitionStatements[i] = currentStatement;
                definitionCount++;
            }
        }
        return definitionStatements[..definitionCount];
    }

    private static int TryGetDefinitionStatementInstance(out DefinitionStatement returnStatement, ReadOnlySpan<DefinitionStatement> definitionStatements,
        uint instanceIdentificator)
    {
        int definitionsLength = definitionStatements.Length;
        for (int i = 0; i < definitionsLength; i++)
        {
            var currentDefinitionNode = (DefinitionNode)definitionStatements[i].ChildNodes.LeftNode;
            uint currentDefinitionIdentifactor = MemoryData.CalculateIdentifierId(currentDefinitionNode.DefinitionIdentifier.NodeData);
            if (currentDefinitionIdentifactor == instanceIdentificator) 
            {
                returnStatement = definitionStatements[i];
                return i;
            }
        }

        returnStatement = null!;
        return -1;
    }

    private static Span<T> ReallocateOnSpan<T>(Span<T> values, int index) 
    {
        if (index < 0 || index > values.Length)
            return values;
        values[index] = values[0];
        return values[1..];
    }
}
