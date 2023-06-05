using Athena.NET.Compilation.Instructions.Definition;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Nodes.Statements;

namespace Athena.NET.Compilation.Instructions;

internal sealed class ReturnInstruction : IInstruction<ReturnStatement>
{
    public bool EmitInstruction(ReturnStatement node, InstructionWriter writer)
    {
        MemoryData returnDefinitionData = DefinitionHelper.GetDefinitionReturnData()
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer) =>
        false;
}
