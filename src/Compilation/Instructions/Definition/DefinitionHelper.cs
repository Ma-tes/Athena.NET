using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Instructions.Structures;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Statements;
using Athena.NET.Parsing.Nodes.Statements.Body;

namespace Athena.NET.Compilation.Instructions.Definition;

/// <summary>
/// Extension helper class for all uses for <see cref="DefinitionStatement"/>,
/// <see cref="DefinitionInstruction"/> or even <see cref="DefinitionCallInstruction"/>.
/// </summary>
internal static class DefinitionHelper
{
    /// <summary>
    /// Provides calculation of definition call order,
    /// that start with index of main definition.
    /// </summary>
    /// <returns>
    /// Order indexes of definitions.
    /// </returns>
    public static ReadOnlyMemory<int> CreateDefinitionsCallOrder(ReadOnlySpan<INode> nodes)
    {
        Span<DefinitionStatement> definitionStatements = GetDefinitionStatements(nodes);
        int mainStatementIndex = TryGetDefinitionStatementInstance(out DefinitionStatement mainDefinitionStatement, definitionStatements,
            InstructionWriter.MainDefinitionIdentificator);
        if (mainStatementIndex == -1)
            return ReadOnlyMemory<int>.Empty;

        Memory<int> returnCallOrder = new int[definitionStatements.Length];
        returnCallOrder.Span[0] = mainStatementIndex;

        Span<int> currentRelativeCallOrder = CreateRelativeCallOrder(mainDefinitionStatement, ref definitionStatements, definitionStatements, mainStatementIndex);
        currentRelativeCallOrder.CopyTo(returnCallOrder.Span[1..]);
        return returnCallOrder;
    }

    /// <summary>
    /// Provides a pre-interpretation of <paramref name="definitionNodes"/>,
    /// with coresponding <paramref name="definitionsData"/> and <paramref name="argumentsData"/>.
    /// </summary>
    /// <returns>
    /// Count of instructions from pre-intepreted <paramref name="definitionNodes"/>.
    /// </returns>
    public static int CalculateDefinitionLength(ReadOnlySpan<INode> definitionNodes, ReadOnlyMemory<MemoryData> argumentsData,
        ReadOnlyMemory<DefinitionData> definitionsData)
    {
        using var definitionInstructionWriter = new InstructionWriter();
        definitionInstructionWriter.InstructionDefinitionData = definitionsData;
        definitionInstructionWriter.TemporaryRegisterTM.memoryData.AddRange(argumentsData.Span);

        definitionInstructionWriter.CreateInstructions(definitionNodes);
        return definitionInstructionWriter.InstructionList.Count;
    }

    /// <summary>
    /// Provides getting or creating <see cref="MemoryData"/>,
    /// from related <paramref name="definitionData"/>.
    /// </summary>
    public static MemoryData GetDefinitionReturnData(DefinitionData definitionData, InstructionWriter instructionWriter)
    {
        uint returnIdentificator = GetDefinitionReturnIdentificator(definitionData.Identificator);
        if (!instructionWriter.TemporaryRegisterTM.TryGetMemoryData(out MemoryData returnMemoryData,
            returnIdentificator))
            returnMemoryData = instructionWriter.TemporaryRegisterTM.AddRegisterData(returnIdentificator, 16);
        return returnMemoryData;
    }

    //TODO: Try to avoid coping from multiple Spans.
    /// <summary>
    /// Creates identifier for <see cref="Register"/>, that's
    /// going to be used for saving specific data, of related definition.
    /// </summary>
    /// <param name="definitionIdentificator">Related definition identificator.</param>
    /// <returns>
    /// Combinated <paramref name="definitionIdentificator"/> with constant
    /// <see cref="returnDefinitionSequence"/>.
    /// </returns>
    private static uint GetDefinitionReturnIdentificator(uint definitionIdentificator) =>
        (definitionIdentificator >> 1) + 209;

    /// <summary>
    /// Creates definition call order and reallocates
    /// <paramref name="definitionStatements"/> by <paramref name="relativeIndex"/>.
    /// </summary> 
    /// <returns>
    /// Order indexes of definitions.
    /// </returns>
    private static Span<int> CreateRelativeCallOrder(DefinitionStatement definitionStatement,
        ref Span<DefinitionStatement> definitionStatements, ReadOnlySpan<DefinitionStatement> originalStatements, int relativeIndex)
    {
        BodyNode definitionBodyNodes = (BodyNode)definitionStatement.ChildNodes.RightNode;
        definitionStatements = ReallocateOnSpan(definitionStatements, relativeIndex);
        return GetDefinitionCallOrder(definitionStatements, originalStatements, definitionBodyNodes.NodeData.Span);
    }

    /// <summary>
    /// Provides exact order of <see cref="CallStatement"/>, which are
    /// then related as a <see cref="DefinitionStatement"/> and recursivly 
    /// executed again.
    /// </summary>
    /// <returns>
    /// Order indexes of definitions.
    /// </returns>
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
                int originalStatementIndex = TryGetDefinitionStatementInstance(out _, originalStatements, identifierId);
                if (currentStatementIndex != -1)
                {
                    Span<int> currentRelativeCallOrder = CreateRelativeCallOrder(currentDefinition, ref definitionStatements, originalStatements, currentStatementIndex);
                    definitionStatements = ReallocateOnSpan(definitionStatements, currentStatementIndex);

                    definitionCallOrderList.AddRange(currentRelativeCallOrder);
                    definitionCallOrderList.Add(originalStatementIndex);
                }
            }
        }
        return definitionCallOrderList.Span;
    }

    /// <summary>
    /// Creates parsing for every <see cref="INode"/> in <paramref name="nodes"/>,
    /// if it's <see cref="DefinitionStatement"/>, otherwise the current <see cref="INode"/>
    /// is ignored.
    /// </summary>
    /// <returns>
    /// Every <see cref="DefinitionStatement"/>, that was found.
    /// </returns>
    public static Span<DefinitionStatement> GetDefinitionStatements(ReadOnlySpan<INode> nodes)
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

    /// <summary>
    /// Tries to find matching <see cref="DefinitionStatement"/>, by comperessing 
    /// <paramref name="instanceIdentificator"/> and <see cref="DefinitionStatement"/> id.
    /// </summary>
    /// <returns>
    /// Relative index, of found <see cref="DefinitionStatement"/> in <paramref name="definitionStatements"/>.
    /// </returns>
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

    //TODO: Create more efficient implementation
    /// <summary>
    /// Creates reallocated <see cref="Span{T}"/>, by swaping
    /// and removing value on <paramref name="index"/>.
    /// </summary>
    private static Span<T> ReallocateOnSpan<T>(Span<T> values, int index) 
    {
        if (values.Length == 1 && index == 0 || values.IsEmpty)
            return Span<T>.Empty;
        if (index < 0 || index > values.Length)
            return values;
        values[index] = values[0];
        return values[1..];
    }
}
