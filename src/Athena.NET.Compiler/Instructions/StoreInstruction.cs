using Athena.NET.Parser.Nodes.StatementNodes;

namespace Athena.NET.Compiler.Instructions
{
    internal sealed class StoreInstruction : IInstruction<EqualAssignStatement>
    {
        public void EmitInstruction(EqualAssignStatement node, InstructionWriter writer)
        {
        }
    }
}
