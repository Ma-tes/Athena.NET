using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Instructions.Structures;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.Parsing.Nodes.Statements;
using Athena.NET.Parsing.Nodes.Statements.Body;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Compilation.Instructions;

/// <summary>
/// Provides generation of raw byte code, 
/// by <see cref="OperatorCodes"/> (custom instructions).
/// </summary>
/// <remarks>
/// This implementation isn't that fully autonomous,
/// because it requires adding every single
/// <see cref="Register"/> to individual methods.<br/>
/// Due to the compilation speed of reflection, 
/// this class must be hand coded as much as possible.
/// </remarks>
public sealed class InstructionWriter : IDisposable
{
    //TODO: Consider a better reimplemtentation
    public static readonly uint MainDefinitionIdentificator = MemoryData.CalculateIdentifierId(
            new char[] { 'M', 'a', 'i', 'n' }
        );
    /// <summary>
    /// Implementation of a <see cref="Register"/> class as
    /// a 8-bit register with a code <see cref="OperatorCodes.AH"/>.
    /// </summary>
    internal Register RegisterAH { get; }
        = new(OperatorCodes.AH, typeof(byte));
    /// <summary>
    /// Implementation of a <see cref="Register"/> class as
    /// a 16-bit register with a code <see cref="OperatorCodes.AX"/>.
    /// </summary>
    internal Register RegisterAX { get; }
        = new(OperatorCodes.AX, typeof(short));
    /// <summary>
    /// Implementation of a <see cref="Register"/> class as
    /// a 16-bit temporary register with a code <see cref="OperatorCodes.TM"/>.
    /// </summary>
    internal Register TemporaryRegisterTM { get; }
        = new(OperatorCodes.TM, typeof(short));

    /// <summary>
    /// It's being used for storing individual
    /// instructions as an <see cref="uint"/> in
    /// a <see cref="NativeMemoryList{T}"/>.
    /// </summary>
    public NativeMemoryList<uint> InstructionList { get; }
        = new();

    //TODO: Improve storing
    /// <summary>
    /// It's being used for storing individual
    /// definition data as an <see cref="InstructionDefinitionData{T}"/> in
    /// a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public ReadOnlyMemory<DefinitionData> InstructionDefinitionData { get; internal set; }
    public DefinitionData MainDefinitionData { get; private set; }

    /// <summary>
    /// Creates individual instructions
    /// from nodes, which are then stored
    /// in an <see cref="InstructionList"/>.
    /// </summary>
    public InstructionWriter(ReadOnlySpan<INode> nodes) 
    {
        if (TryGetDefinitionsData(out ReadOnlyMemory<DefinitionData> returnData, nodes))
            InstructionDefinitionData = returnData;
    }

    //TODO: Better exception and error handling
    public void CreateInstructions(ReadOnlySpan<INode> nodes)
    {
        if(!TryGetDefinitionData(out DefinitionData mainDefinitionData, MainDefinitionIdentificator))
            throw new Exception("Main definition wasn't found");
        MainDefinitionData = mainDefinitionData;

        int nodesLength = nodes.Length;
        for (int i = 0; i < nodesLength; i++)
        {
            if (!TryGetEmitInstruction(nodes[i]))
                throw new Exception("Instruction wasn't completed or found");
        }
    }

    private bool TryGetDefinitionsData([NotNullWhen(true)]out ReadOnlyMemory<DefinitionData> returnDefinitions, ReadOnlySpan<INode> nodes)
    {
        int nodesLength = nodes.Length;
        Memory<DefinitionData> currentDefinitions = new DefinitionData[nodesLength];
        Span<DefinitionData> currentDefinitionsSpan = currentDefinitions.Span;
        for (int i = 0; i < nodesLength; i++)
        {
            if (nodes[i] is not DefinitionStatement definitionStatement) 
            {
                returnDefinitions = null;
                return false;
            }
            DefinitionNode leftDefinitionNode = (DefinitionNode)definitionStatement.ChildNodes.LeftNode;
            int definitionMemoryDataLength = leftDefinitionNode.NodeToken != Lexing.TokenIndentificator.Unknown ?
                leftDefinitionNode.NodeData.Length + 1 : leftDefinitionNode.NodeData.Length;
            uint definitionIdentificator = MemoryData.CalculateIdentifierId(leftDefinitionNode.DefinitionIdentifier.NodeData);

            int definitionBodyLenght = leftDefinitionNode.NodeToken == Lexing.TokenIndentificator.Unknown &&
                 definitionIdentificator != MainDefinitionIdentificator ? 5 : 0;
            currentDefinitionsSpan[i] = new DefinitionData(
                    definitionIdentificator,
                    (definitionMemoryDataLength * 6), definitionBodyLenght,
                    GetArgumentsMemoryData(leftDefinitionNode.NodeData),
                    (BodyNode)definitionStatement.ChildNodes.RightNode, default
                );
        }
        returnDefinitions = CreateRelativeDefinitionsData(currentDefinitions);
        return true;
    }

    private ReadOnlyMemory<DefinitionData> CreateRelativeDefinitionsData(Memory<DefinitionData> definitionsData)
    {
        int definitionInstructionCount = 0;
        int definitionsDataLength = definitionsData.Length;
        for (int i = 0; i < definitionsDataLength; i++)
        {
            ref DefinitionData currentDefinitionData = ref definitionsData.Span[i];
            ReadOnlySpan<INode> currentBodyNodes = currentDefinitionData.DefinitionBodyNode.NodeData.Span;

            int argumentsIntructionLength = currentDefinitionData.DefinitionIndex;
            int definitionBodyLength = CalculateDefinitionLength(currentBodyNodes, currentDefinitionData.DefinitionArguments, definitionsData);

            currentDefinitionData.DefinitionIndex += definitionInstructionCount;
            currentDefinitionData.DefinitionLength += definitionBodyLength;
            definitionInstructionCount += ((currentDefinitionData.DefinitionLength) + argumentsIntructionLength);
        }
        return definitionsData;
    }

    private ReadOnlyMemory<MemoryData> GetArgumentsMemoryData(ReadOnlyMemory<InstanceNode> argumentInstances)
    {
        int instancesLength = argumentInstances.Length;
        if (instancesLength == 0)
            return null;

        ReadOnlySpan<InstanceNode> instancesSpan = argumentInstances.Span;
        Memory<MemoryData> returnRegisters = new MemoryData[instancesLength];
        for (int i = 0; i < instancesLength; i++)
        {
            ReadOnlyMemory<char> argumentIdentificator = instancesSpan[i].NodeData;
            MemoryData argumentMemoryData = TemporaryRegisterTM.AddRegisterData(argumentIdentificator, 16);
            returnRegisters.Span[i] = argumentMemoryData;
        }
        return returnRegisters;
    }

    private static int CalculateDefinitionLength(ReadOnlySpan<INode> definitionNodes, ReadOnlyMemory<MemoryData> argumentsData,
        ReadOnlyMemory<DefinitionData> returnDefinitions)
    {
        using var definitionInstructionWriter = new InstructionWriter(definitionNodes);
        definitionInstructionWriter.InstructionDefinitionData = returnDefinitions;
        definitionInstructionWriter.TemporaryRegisterTM.memoryData.AddRange(argumentsData.Span);

        definitionInstructionWriter.CreateInstructions(definitionNodes);
        return definitionInstructionWriter.InstructionList.Count;
    }

    /// <summary>
    /// Executes a related instruction to a specific 
    /// node that was derived from <see cref="INode"/>.
    /// </summary>
    /// <returns>
    /// Specific <see cref="bool"/> state of a
    /// <see cref="IInstruction{T}.EmitInstruction(T, InstructionWriter)"/>
    /// <see langword="where"/> T : <see cref="INode"/>.
    /// </returns>
    private bool TryGetEmitInstruction(INode node) => node switch
    {
        EqualAssignStatement equalNode => new StoreInstruction()
            .EmitInstruction(equalNode, this),
        PrintStatement printNode => new PrintInstruction()
            .EmitInstruction(printNode, this),
        IfStatement ifNode => new JumpInstruction()
            .EmitInstruction(ifNode, this),
        DefinitionStatement definitionNode => new DefinitionInstruction()
            .EmitInstruction(definitionNode, this),
        CallStatement definitionCallNode => new DefinitionCallInstruction()
            .EmitInstruction(definitionCallNode, this),
        OperatorNode operatorNode => new OperatorInstruction()
            .EmitInstruction(operatorNode, this),
        _ => false
    };

    /// <summary>
    /// Chooses a matching <see cref="Register"/> from current
    /// <see cref="InstructionWriter"/> by size of <paramref name="data"/>.
    /// </summary>
    /// <returns>Specific <see cref="Register"/> for current <paramref name="data"/> size.</returns>
    internal Register? GetEmitIntRegister(int data)
    {
        if (RegisterAH.CalculateByteSize(data) != RegisterAH.TypeSize) { return RegisterAH; }
        if (RegisterAX.CalculateByteSize(data) != RegisterAX.TypeSize) { return RegisterAX; }
        return null;
    }

    /// <summary>
    /// Chooses a matching <see cref="Register"/> and <see cref="MemoryData"/>
    /// from current <see cref="InstructionWriter"/> by <paramref name="identifierId"/>.
    /// </summary>
    /// <returns>Specific <see cref="Register"/> and coresponding
    /// <see langword="out"/> <see cref="MemoryData"/>.
    /// </returns>
    internal Register? GetIdentifierData(out MemoryData returnData, uint identifierId)
    {
        if (RegisterAH.TryGetMemoryData(out MemoryData AHData, identifierId)) { returnData = AHData; return RegisterAH; }
        if (RegisterAX.TryGetMemoryData(out MemoryData AXData, identifierId)) { returnData = AXData; return RegisterAX; }
        if (TemporaryRegisterTM.TryGetMemoryData(out MemoryData TMData, identifierId)) { returnData = TMData; return TemporaryRegisterTM; }
        returnData = default!;
        return null;
    }

    /// <summary>
    /// Chooses a matching <see cref="Register"/> and <see cref="MemoryData"/>
    /// from current <see cref="InstructionWriter"/> by <paramref name="identifierName"/>.
    /// </summary>
    /// <returns>Specific <see cref="Register"/> and coresponding
    /// <see langword="out"/> <see cref="MemoryData"/>.
    /// </returns>
    internal Register? GetIdentifierData(out MemoryData returnData, ReadOnlyMemory<char> identifierName)
    {
        uint identiferId = MemoryData.CalculateIdentifierId(identifierName);
        return GetIdentifierData(out returnData, identiferId);
    }

    internal bool TryGetDefinitionData([NotNullWhen(true)] out DefinitionData returnData, uint definitionIdentificator) 
    {
        int definitionDataCount = InstructionDefinitionData.Length;
        for (int i = 0; i < definitionDataCount; i++)
        {
            DefinitionData currentDefinitionData = InstructionDefinitionData.Span[i];
            if (currentDefinitionData.Identificator == definitionIdentificator)
            {
                returnData = currentDefinitionData;
                return true;
            }
        }
        returnData = default;
        return false;
    }

    internal void AddMemoryDataInstructions(OperatorCodes registerCode, MemoryData memoryData)
    {
        InstructionList.AddRange((uint)registerCode,
            (uint)memoryData.Size,
            (uint)memoryData.Offset);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        RegisterAH.Dispose();
        RegisterAX.Dispose();
        InstructionList.Dispose();
    }
}
