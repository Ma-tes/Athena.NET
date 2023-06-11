using Athena.NET.Compilation.Interpretation;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Compilation.Instructions;

/// <summary>
/// Definition of specific instruction structure, 
/// that's based on <typeparamref name="T"/> Node.
/// </summary>
/// <typeparam name="T">
/// Node that must be implemented from <see cref="INode"/>.
/// </typeparam>
internal interface IInstruction<T> where T : INode
{
    /// <summary>
    /// Creates conversion of <paramref name="node"/> data into
    /// <see cref="OperatorCodes"/> and saves new <see cref="Register"/> data.
    /// <br/>All instructions are saved in <see cref="InstructionWriter.InstructionList"/>.
    /// </summary>
    /// <param name="node">Specified node, for instruction conversion.</param>
    /// <param name="writer">Handles and stores all instruction data.</param>
    /// <returns>State of emitted <paramref name="node"/></returns>
    public bool EmitInstruction(T node, InstructionWriter writer);

    /// <summary>
    /// Creates final interpretation of <paramref name="instructions"/>, that
    /// are related to <typeparamref name="T"/> Node. Register saving is provided,
    /// by <see cref="VirtualMachine.virtualRegisters"/>.
    /// </summary>
    /// <param name="instructions">Specified <see langword="uint"/> values for interpretation.</param>
    /// <param name="writer">Provides handeling all <see cref="RegisterMemory"/>, with coresponding
    /// separation of other instruction.</param>
    /// <returns></returns>
    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer);
}
