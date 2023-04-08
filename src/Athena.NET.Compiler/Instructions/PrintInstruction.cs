using Athena.NET.Compiler.Interpreter;
using Athena.NET.Compiler.Structures;
using Athena.NET.Parser.Nodes;
using Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes;

namespace Athena.NET.Compiler.Instructions
{
    internal sealed class PrintInstruction : IInstruction<PrintStatement>
    {
        public bool EmitInstruction(PrintStatement node, InstructionWriter writer) 
        {
            var rightChildrenNode = node.ChildNodes.RightNode;
            if (rightChildrenNode is OperatorNode operatorNode) 
            {
                var operatorInstruction = new OperatorInstruction();
                if (!operatorInstruction.EmitInstruction(operatorNode, writer))
                    return false;
                writer.InstructionList.AddRange((uint)OperatorCodes.Nop, (uint)OperatorCodes.Print);
                writer.AddMemoryDataInstructions(OperatorCodes.TM, operatorInstruction.EmitMemoryData);
                return true;
            }

            writer.InstructionList.AddRange((uint)OperatorCodes.Nop, (uint)OperatorCodes.Print);
            if (rightChildrenNode is IdentifierNode identifierNode) 
            {
                Register? idetifierRegister = writer.GetIdentifierData(out MemoryData identifierData, identifierNode.NodeData);
                if (idetifierRegister is null)
                    return false;
                writer.AddMemoryDataInstructions(idetifierRegister.RegisterCode, identifierData);
                return true; 
            }
            writer.InstructionList.Add((uint)((DataNode<int>)rightChildrenNode).NodeData);
            return true;
        }

        public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer) 
        {
            int currentData = writer.GetInstructionData(instructions[1..])[0];
            Console.WriteLine(currentData);
            return true;
        }
    }
}
