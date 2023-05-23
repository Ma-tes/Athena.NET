using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Instructions.Definition;
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
    /// <summary>
    /// Identificatora of a Main <see cref="Definition"/>.
    /// </summary>
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

    /// <summary>
    /// It's being used for storing individual
    /// definition data as an <see cref="InstructionDefinitionData{T}"/> in
    /// a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public ReadOnlyMemory<DefinitionData> InstructionDefinitionData { get; internal set; }

    /// <summary>
    /// It's being used for storing definition
    /// call order as an <see cref="int"/> in
    /// a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public ReadOnlyMemory<int> InstructionDefinitionOrder { get; internal set; }

    /// <summary>
    /// Provide's <see cref="DefinitionData"/> of
    /// first definitions, which identificator is equal
    /// to <see cref="MainDefinitionIdentificator"/>.
    /// </summary>
    public DefinitionData MainDefinitionData { get; private set; }

    public InstructionWriter() { }
    public InstructionWriter(ReadOnlySpan<INode> nodes)
    {
        InstructionDefinitionOrder = DefinitionHelper.CreateDefinitionsCallOrder(nodes);
        InstructionDefinitionData = GetDefinitionsData(nodes);
    }

    //TODO: Better exception and error handling
    /// <summary>
    /// Creates individual instructions
    /// from <paramref name="nodes"/>, which are then stored
    /// in an <see cref="InstructionList"/>.
    /// </summary>
    public void CreateInstructions(ReadOnlySpan<INode> nodes)
    {
        ReadOnlySpan<DefinitionStatement> definitionStatements = DefinitionHelper.GetDefinitionStatements(nodes);
        if(TryGetDefinitionData(out DefinitionData mainDefinitionData, MainDefinitionIdentificator))
            MainDefinitionData = mainDefinitionData;

        ReadOnlySpan<int> definitionOrderIndexes = InstructionDefinitionOrder.Span;
        int nodesLength = nodes.Length;
        for (int i = 0; i < nodesLength; i++)
        {
            int definitionIndex = InstructionDefinitionOrder.Length != 0 && !definitionStatements.IsEmpty
                ? definitionOrderIndexes[i] : i;
            if (!TryGetEmitInstruction(nodes[definitionIndex]))
                throw new Exception("Instruction wasn't completed or found");
        }
    }

    /// <summary>
    /// Tries to get <see cref="DefinitionData"/> from <paramref name="nodes"/>,
    /// which are pre-calculated for future instruction use
    /// </summary>
    private ReadOnlyMemory<DefinitionData> GetDefinitionsData(ReadOnlySpan<INode> nodes)
    {
        int nodesLength = nodes.Length;
        Memory<DefinitionData> currentDefinitions = new DefinitionData[nodesLength];

        Span<DefinitionData> currentDefinitionsSpan = currentDefinitions.Span;
        ReadOnlySpan<int> definitionOrderIndexes = InstructionDefinitionOrder.Span;
        for (int i = 0; i < nodesLength; i++)
        {
            int currentDefinitionIndex = definitionOrderIndexes[i];
            DefinitionNode leftDefinitionNode = (DefinitionNode)nodes[currentDefinitionIndex].ChildNodes.LeftNode;

            int definitionMemoryDataLength = leftDefinitionNode.NodeToken != Lexing.TokenIndentificator.Unknown ?
                leftDefinitionNode.NodeData.Length + 1 : leftDefinitionNode.NodeData.Length;
            uint definitionIdentificator = MemoryData.CalculateIdentifierId(leftDefinitionNode.DefinitionIdentifier.NodeData);

            int definitionBodyLenght = leftDefinitionNode.NodeToken == Lexing.TokenIndentificator.Unknown &&
                 definitionIdentificator != MainDefinitionIdentificator ? 5 : 0;
            currentDefinitionsSpan[i] = new DefinitionData(
                    definitionIdentificator,
                    (definitionMemoryDataLength * 6), definitionBodyLenght,
                    GetArgumentsMemoryData(leftDefinitionNode.NodeData),
                    (BodyNode)nodes[currentDefinitionIndex].ChildNodes.RightNode, default
                );
        }
        return CreateRelativeDefinitionsData(currentDefinitions);
    }

    /// <summary>
    /// Calculates relative size of every <see cref="DefinitionData"/>,
    /// which is then saved in the same <see cref="DefinitionData"/>
    /// </summary>
    private ReadOnlyMemory<DefinitionData> CreateRelativeDefinitionsData(Memory<DefinitionData> definitionsData)
    {
        int definitionInstructionCount = 0;
        int definitionsDataLength = definitionsData.Length;
        for (int i = 0; i < definitionsDataLength; i++)
        {
            ref DefinitionData currentDefinitionData = ref definitionsData.Span[i];
            ReadOnlySpan<INode> currentBodyNodes = currentDefinitionData.DefinitionBodyNode.NodeData.Span;

            int argumentsIntructionLength = currentDefinitionData.DefinitionIndex;
            int definitionBodyLength = DefinitionHelper.CalculateDefinitionLength(currentBodyNodes, currentDefinitionData.DefinitionArguments, definitionsData);

            currentDefinitionData.DefinitionIndex += definitionInstructionCount;
            currentDefinitionData.DefinitionLength += definitionBodyLength;
            definitionInstructionCount += (currentDefinitionData.DefinitionLength + argumentsIntructionLength);
        }
        return definitionsData;
    }

    /// <summary>
    /// Creates <see cref="MemoryData"/> from <paramref name="argumentInstances"/>,
    /// which are stored in <see cref="OperatorCodes.TM"/> <see cref="Register"/>.
    /// </summary>
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
        EqualAssignStatement equalNode => InstructionFactory.StoreInstruction
            .EmitInstruction(equalNode, this),
        PrintStatement printNode => InstructionFactory.PrintInstruction
            .EmitInstruction(printNode, this),
        IfStatement ifNode => InstructionFactory.JumpInstruction
            .EmitInstruction(ifNode, this),
        DefinitionStatement definitionNode => InstructionFactory.DefinitionInstruction
            .EmitInstruction(definitionNode, this),
        CallStatement definitionCallNode => InstructionFactory.DefinitionCallInstruction
            .EmitInstruction(definitionCallNode, this),
        OperatorNode operatorNode => InstructionFactory.OperatorInstruction
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

    /// <summary>
    /// Tries to get <see cref="DefinitionData"/>, by finding
    /// <see cref="DefinitionData.Identificator"/>, that's
    /// equal to <paramref name="definitionIdentificator"/>.
    /// </summary>
    /// <returns>Bool state if <see cref="DefinitionData"/> was found.</returns>
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
        return NullableHelper.NullableOutValue(out returnData);
    }

    /// <summary>
    /// Adds <see cref="MemoryData.Size"/> and <see cref="MemoryData.Offset"/>,
    /// from <paramref name="memoryData"/>, instructions to <see cref="InstructionList"/>
    /// with coresponding <paramref name="registerCode"/> instruction.
    /// </summary>
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
