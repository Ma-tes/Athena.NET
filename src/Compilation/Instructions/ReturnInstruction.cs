using Athena.NET.Compilation.Instructions.Definition;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Statements;

namespace Athena.NET.Compilation.Instructions;

internal sealed class ReturnInstruction : IInstruction<ReturnStatement>
{
    //TODO: Implement dynamic jumping to call instruction,
    //with a propriete return memory data in register.
    public bool EmitInstruction(ReturnStatement node, InstructionWriter writer)
    {
        MemoryData returnDefinitionData = DefinitionHelper.GetDefinitionReturnData(writer.CurrentDefinitionData, writer);
        INode returnChildrenRightNode = node.ChildNodes.RightNode;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer) =>
        false;
}
