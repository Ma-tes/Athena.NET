using Athena.NET.Athena.NET.Compiler;
using Athena.NET.Athena.NET.Compiler.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.StatementNodes;

namespace Athena.NET.Compiler.Instructions
{
    public sealed class InstructionWriter : IDisposable
    {
        internal Register RegisterAH { get; }
            = new(OperatorCodes.AH, typeof(byte));
        internal Register RegisterAX { get; }
            = new(OperatorCodes.AX, typeof(short));

        public ReadOnlyMemory<INode> Nodes { get; }
        public NativeMemoryList<uint> InstructionList { get; }

        public InstructionWriter(ReadOnlyMemory<INode> nodes)
        {
            Nodes = nodes;
            InstructionList = new NativeMemoryList<uint>();
        }

        //TODO: Change the exception to a proper
        //error message
        public void CreateInstructions()
        {
            ReadOnlySpan<INode> nodesSpan = Nodes.Span;
            int nodesLength = nodesSpan.Length;
            for (int i = 0; i < nodesLength; i++)
            {
                InstructionList.Add((uint)OperatorCodes.Nop);
                if (!TryGetEmitInstruction(nodesSpan[i]))
                    throw new Exception("Instruction wasn't completed or found");
            }
        }

        //I'm not sure if I like this solution,
        //because I could easily create reflection that
        //will find the right instruction without any
        //hard coded values
        private bool TryGetEmitInstruction(INode node) => node switch
        {
            EqualAssignStatement equalNode => new StoreInstruction()
                .EmitInstruction(equalNode, this),
            _ => false
        };

        internal Register? GetEmitIntRegister(int data)
        {
            if (RegisterAH.CalculateByteSize(data) != -1) { return RegisterAH; }
            if (RegisterAX.CalculateByteSize(data) != -1) { return RegisterAX; }
            return null;
        }

        public void Dispose() 
        {
            RegisterAH.Dispose();
            RegisterAX.Dispose();
            InstructionList.Dispose();
        }
    }
}
