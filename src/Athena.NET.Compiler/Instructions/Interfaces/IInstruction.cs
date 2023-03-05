using Athena.NET.Parser.Interfaces;

namespace Athena.NET.Compiler.Instructions
{
    internal interface IInstruction<T> where T : INode
    {
        public void EmitInstruction(T node, InstructionWriter writer);
    }
}
