using Athena.NET.Athena.NET.Compiler.DataHolders;
using Athena.NET.Athena.NET.Compiler.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes;

namespace Athena.NET.Compiler.Instructions
{
    public sealed class InstructionWriter : IDisposable
    {
        internal Register RegisterAH { get; }
            = new(OperatorCodes.AH, typeof(byte));
        internal Register RegisterAX { get; }
            = new(OperatorCodes.AX, typeof(short));
        internal Register TemporaryRegisterTM { get; }
            = new(OperatorCodes.TM, typeof(short));

        public ReadOnlyMemory<INode> Nodes { get; }
        public NativeMemoryList<uint> InstructionList { get; }
            = new();

        public InstructionWriter(ReadOnlyMemory<INode> nodes)
        {
            Nodes = nodes;
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
        //hard code
        private bool TryGetEmitInstruction(INode node) => node switch
        {
            EqualAssignStatement equalNode => new StoreInstruction()
                .EmitInstruction(equalNode, this),
            OperatorNode operatorNode => new OperatorInstruction()
                .EmitInstruction(operatorNode, this),
            _ => false
        };

        internal Register? GetEmitIntRegister(int data)
        {
            if (RegisterAH.CalculateByteSize(data) != -1) { return RegisterAH; }
            if (RegisterAX.CalculateByteSize(data) != -1) { return RegisterAX; }
            return null;
        }

        internal Register? GetIdentifierData(out MemoryData returnData, ReadOnlyMemory<char> identifierName)
        {
            if (RegisterAH.TryGetMemoryData(out MemoryData AHData, identifierName)) { returnData = AHData; return RegisterAH; }
            if (RegisterAX.TryGetMemoryData(out MemoryData AXData, identifierName)) { returnData = AXData; return RegisterAX; }
            returnData = default!;
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
