using Athena.NET.Compilation.Interpreter;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Compilation.Instructions;

//TODO: After switch to .NET 7, consider
//implementation of factory pattern, for
//this interface
internal interface IInstruction<T> where T : INode
{
    public bool EmitInstruction(T node, InstructionWriter writer);
    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer);
}
