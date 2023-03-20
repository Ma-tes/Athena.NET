using Athena.NET.Compiler.DataHolders;
using Athena.NET.Compiler.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes;
using Athena.NET.Parser.Nodes.StatementNodes.BodyStatements;

namespace Athena.NET.Compiler.Instructions
{
    public sealed class InstructionWriter : IDisposable
    {
        internal Register RegisterAH { get; }
            = new(OperatorCodes.AH, typeof(byte));
        internal Register RegisterAX { get; }
            = new(OperatorCodes.AX, typeof(short));

        internal Register RegisterEAX { get; }
            = new(OperatorCodes.EAX, typeof(int));

        internal Register TemporaryRegisterTM { get; }
            = new(OperatorCodes.TM, typeof(short));

        public NativeMemoryList<uint> InstructionList { get; }
            = new();


        //TODO: Change the exception to a proper
        //error message
        public void CreateInstructions(ReadOnlySpan<INode> nodes)
        {
            int nodesLength = nodes.Length;
            for (int i = 0; i < nodesLength; i++)
            {
                InstructionList.Add((uint)OperatorCodes.Nop);
                if (!TryGetEmitInstruction(nodes[i]))
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
            BodyStatement bodyNode => new JumpInstruction()
                .EmitInstruction(bodyNode, this),
            OperatorNode operatorNode => new OperatorInstruction()
                .EmitInstruction(operatorNode, this),
            _ => false
        };

        internal Register? GetEmitIntRegister(int data)
        {
            if (RegisterAH.CalculateByteSize(data) != RegisterAH.TypeSize) { return RegisterAH; }
            if (RegisterAX.CalculateByteSize(data) != RegisterAX.TypeSize) { return RegisterAX; }
            return null;
        }

        internal Register? GetIdentifierData(out MemoryData returnData, uint identifierId)
        {
            if (RegisterAH.TryGetMemoryData(out MemoryData AHData, identifierId)) { returnData = AHData; return RegisterAH; }
            if (RegisterAX.TryGetMemoryData(out MemoryData AXData, identifierId)) { returnData = AXData; return RegisterAX; }
            returnData = default!;
            return null;
        }

        internal Register? GetIdentifierData(out MemoryData returnData, ReadOnlyMemory<char> identifierName)
        {
            uint identiferId = MemoryData.CalculateIdentifierId(identifierName);
            return GetIdentifierData(out returnData, identiferId);
        }

        public void Dispose()
        {
            RegisterAH.Dispose();
            RegisterAX.Dispose();
            InstructionList.Dispose();
        }
    }
}
