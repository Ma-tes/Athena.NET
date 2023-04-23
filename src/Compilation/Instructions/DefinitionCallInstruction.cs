using Athena.NET.Compilation.Interpreter;
using Athena.NET.Parsing.Nodes.Statements;

namespace Athena.NET.Compilation.Instructions;

internal sealed class DefinitionCallInstruction : IInstruction<CallStatement>
{
    public bool EmitInstruction(CallStatement node, InstructionWriter writer) 
    {
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
    }
}
