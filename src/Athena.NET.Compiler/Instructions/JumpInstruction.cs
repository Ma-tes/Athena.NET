using Athena.NET.Compiler.Structures;
using Athena.NET.Lexer;
using Athena.NET.Parser;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes;
using Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes.BodyStatements;

namespace Athena.NET.Compiler.Instructions
{
    internal sealed class JumpInstruction : IInstruction<BodyStatement>
    {
        public bool EmitInstruction(BodyStatement node, InstructionWriter writer) 
        {
            ChildrenNodes leftChildrenNodes = node.ChildNodes.LeftNode.ChildNodes;
            MemoryData leftData = GetMemoryData(leftChildrenNodes.LeftNode, writer);
            MemoryData rightData = GetMemoryData(leftChildrenNodes.RightNode, writer);

            writer.InstructionList.AddRange((uint)OperatorCodes.Nop,
                (uint)GetJumpOperatorCode((OperatorNode)node.ChildNodes.LeftNode));
            WriteMemoryDataInstruction(leftData, leftChildrenNodes.LeftNode, writer);
            WriteMemoryDataInstruction(rightData, leftChildrenNodes.RightNode, writer);

            BodyNode bodyNode = (BodyNode)node.ChildNodes.RightNode;
            writer.InstructionList.AddRange((uint)OperatorCodes.Goto,
                (uint)bodyNode.NodeData.Length);
            writer.CreateInstructions(bodyNode.NodeData);
            return true;
        }

        private void WriteMemoryDataInstruction(MemoryData memoryData, INode node, InstructionWriter writer) 
        {
            if (memoryData.Equals(default) && node is DataNode<int> dataNode) 
            {
                writer.InstructionList.Add((uint)dataNode.NodeData);
                return;
            }

            Register? memoryDataRegister = writer.GetIdentifierData(out _, memoryData.IdentifierId);
            OperatorCodes registerCode = memoryDataRegister is not null ?
                memoryDataRegister.RegisterCode : OperatorCodes.TM;

            writer.InstructionList.AddRange((uint)memoryData.Offset,
                (uint)memoryData.Size,
                (uint)registerCode);
        }

        private MemoryData GetMemoryData(INode node, InstructionWriter writer)
        {
            if (node is OperatorNode operatorNode) 
            {
                var currentInstruction = new OperatorInstruction();
                currentInstruction.EmitInstruction(operatorNode, writer);
                return currentInstruction.EmitMemoryData;
            }

            if (node is IdentifierNode identifierNode) 
            {
                _ = writer.GetIdentifierData(out MemoryData returnData, identifierNode.NodeData);
                return returnData;
            }
            return default;
        }

        private OperatorCodes GetJumpOperatorCode(OperatorNode node) => node.NodeToken switch
        {
            TokenIndentificator.EqualLogical => OperatorCodes.JumpNE,
            TokenIndentificator.NotEqual => OperatorCodes.JumpE,
            TokenIndentificator.GreaterThan => OperatorCodes.JumpL,
            TokenIndentificator.GreaterEqual => OperatorCodes.JumpLE,
            TokenIndentificator.LessThan => OperatorCodes.JumpG,
            TokenIndentificator.LessEqual => OperatorCodes.JumpGE,
            _ => default
        };
    }
}
