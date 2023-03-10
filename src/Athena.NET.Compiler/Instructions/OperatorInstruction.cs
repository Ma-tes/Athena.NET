using Athena.NET.Compiler.Structures;
using Athena.NET.Compiler.DataHolders;
using Athena.NET.Lexer;
using Athena.NET.Parser;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes;
using Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Parser.Nodes.OperatorNodes;

namespace Athena.NET.Compiler.Instructions
{
    internal sealed class OperatorInstruction : IInstruction<OperatorNode>
    {
        private NativeMemoryList<uint> operatorInstructions = new();

        public MemoryData EmitMemoryData { get; private set; }

        public bool EmitInstruction(OperatorNode node, InstructionWriter writer)
        {
            bool isInstruction = TryGenerateOperatorInstructions(out MemoryData returnData, node, writer);
            writer.InstructionList.AddRange(operatorInstructions.Span);
            EmitMemoryData = returnData;

            operatorInstructions.Dispose();
            return isInstruction;
        }

        private bool TryGenerateOperatorInstructions(out MemoryData returnData, INode node, 
            InstructionWriter writer)
        {
            ChildrenNodes childrenNodes = node.ChildNodes; 
            if (node is OperatorNode operatorNode) 
            {
                OperatorCodes instructionOperator = GetOperatorNodeCode(operatorNode);
                var currentInstructions = new NativeMemoryStack<uint>();
                returnData = writer.TemporaryRegisterTM.AddRegisterData(new char[1], 16);

                currentInstructions.AddRange((uint)OperatorCodes.Nop,
                    (uint)instructionOperator,
                    (uint)returnData.Size,
                    (uint)returnData.Offset,
                    (uint)OperatorCodes.TM);

                WriteMemoryDataInstructions(currentInstructions, childrenNodes.LeftNode, writer);
                WriteMemoryDataInstructions(currentInstructions, childrenNodes.RightNode, writer);

                operatorInstructions.AddRange(currentInstructions.Span);
                currentInstructions.Dispose();
                return true;
            }

            if (node is IdentifierNode identifierNode) 
            {
                _ = writer.GetIdentifierData(out returnData, identifierNode.NodeData);
                return true;
            }
            returnData = default;
            return false;
        }

        public void WriteMemoryDataInstructions(NativeMemoryList<uint> nativeInstructions, INode node, InstructionWriter writer) 
        {
            if (TryGenerateOperatorInstructions(out MemoryData returnData, node, writer)) 
            {
                OperatorCodes registerCode = node is IdentifierNode identifierNode ?
                    writer.GetIdentifierData(out _, identifierNode.NodeData)!.RegisterCode :
                    OperatorCodes.TM;
                nativeInstructions.AddRange((uint)returnData.Size,
                    (uint)returnData.Offset,
                    (uint)registerCode);
            }
            else
                nativeInstructions.Add((uint)((DataNode<int>)node).NodeData);
        }

        private OperatorCodes GetOperatorNodeCode(OperatorNode node) => node.NodeToken switch
        {
            TokenIndentificator.Add => OperatorCodes.Add,
            TokenIndentificator.Sub => OperatorCodes.Sub,
            TokenIndentificator.Mul => OperatorCodes.Mul,
            TokenIndentificator.Div => OperatorCodes.Div,
            _ => default
        };
    }
}
