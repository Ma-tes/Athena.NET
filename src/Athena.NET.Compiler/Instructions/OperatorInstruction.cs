using Athena.NET.Athena.NET.Compiler.Structures;
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
        public MemoryData EmitMemoryData { get; private set; }

        public bool EmitInstruction(OperatorNode node, InstructionWriter writer)
        {
            bool isInstruction = TryGenerateOperatorInstructions(out MemoryData returnData, node, writer);
            EmitMemoryData = returnData;
            return isInstruction;
        }

        private bool TryGenerateOperatorInstructions(out MemoryData returnData, INode node, InstructionWriter writer)
        {
            ChildrenNodes childrenNodes = node.ChildNodes; 
            if (node is OperatorNode operatorNode) 
            {
                OperatorCodes instructionOperator = GetOperatorNodeCode(operatorNode);
                writer.InstructionList.Add((uint)instructionOperator);
                writer.InstructionList.Add((uint)OperatorCodes.TM);

                WriteMemoryDataInstructions(childrenNodes.LeftNode, writer);
                WriteMemoryDataInstructions(childrenNodes.RightNode, writer);

                returnData = writer.TemporaryRegisterTM.AddRegisterData(new char[1], 16);
                return true;
            }

            if (node is IdentifierNode identifierNode) 
            {
                _ = writer.GetIdentifierData(out MemoryData memoryData, identifierNode.NodeData);
                returnData = memoryData;
                return true;
            }
            returnData = default;
            return false;
        }

        private void WriteMemoryDataInstructions(INode node, InstructionWriter writer) 
        {
            if (TryGenerateOperatorInstructions(out MemoryData returnData, node, writer)) 
            {
                OperatorCodes registerCode = node is IdentifierNode identifierNode ?
                    writer.GetIdentifierData(out _, identifierNode.NodeData)!.RegisterCode :
                    OperatorCodes.TM;
                writer.InstructionList.Add((uint)registerCode);
                writer.InstructionList.Add((uint)returnData.Size);
                writer.InstructionList.Add((uint)returnData.Offset);
            }
            else
                writer.InstructionList.Add((uint)((DataNode<int>)node).NodeData);
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
