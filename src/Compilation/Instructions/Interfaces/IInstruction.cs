using Athena.NET.Compilation.Interpreter;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Compilation.Instructions;

internal interface IInstruction<T, TSelf> where T : INode where TSelf : IInstruction<T, TSelf>
{
    public static abstract TSelf InstructionInstance { get; }

    public bool EmitInstruction(T node, InstructionWriter writer);
    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer);
}
