using Athena.NET.Compilation.Interpreter;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Compilation.Instructions;

internal interface IInstruction<T> where T : INode
{
    public bool EmitInstruction(T node, InstructionWriter writer);
    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer);
}
