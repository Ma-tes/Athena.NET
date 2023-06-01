using Athena.NET.Compilation.Instructions.Structures;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Parsing.Nodes.Statements;

namespace Athena.NET.Compilation.Instructions;

internal sealed class ReturnInstruction : IInstruction<ReturnStatement>
{
    public bool EmitInstruction(ReturnStatement node, InstructionWriter writer)
    {
        DefinitionData currentDefinitionData = writer.CurrentDefinitionData;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer) =>
        false;
}
