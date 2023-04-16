using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Instructions.Structures;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.Parsing.Nodes.Statements;
using Athena.NET.Parsing.Nodes.Statements.Body;

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
    /// Creates individual instructions
    /// from nodes, which are then stored
    /// in an <see cref="InstructionList"/>.
    /// </summary>
    public void CreateInstructions(ReadOnlySpan<INode> nodes)
    {
        int nodesLength = nodes.Length;
        for (int i = 0; i < nodesLength; i++)
        {
            if (!TryGetEmitInstruction(nodes[i]))
                throw new Exception("Instruction wasn't completed or found");
        }
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
