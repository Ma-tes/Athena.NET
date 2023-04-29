using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Lexing;
using Athena.NET.Parsing;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.Parsing.Nodes.Statements.Body;

namespace Athena.NET.Compilation.Instructions;

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
        writer.InstructionList.Add(0);
        int currentInstructionLength = writer.InstructionList.Count;

        writer.CreateInstructions(bodyNode.NodeData.Span);
        writer.InstructionList.Span[currentInstructionLength - 1] = (uint)(writer.InstructionList.Count - currentInstructionLength);
        return true;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
        if (instructions.Length <= 1)
            return false;
        TokenIndentificator operatorInstruction = (TokenIndentificator)((instructions[0] ^ (0xffeec << 4)) + 5);
        if (!OperatorHelper.TryGetOperator(out OperatorNode instructionNode, operatorInstruction))
        {
            writer.LastInstructionNopIndex += (int)instructions[1];
            return true;
        }

        var operatorData = writer.GetInstructionData(instructions[1..]);
        int operatorResult = instructionNode.CalculateData(operatorData[0], operatorData[1]);
        if (operatorResult == 1)
            writer.LastInstructionNopIndex += operatorData[2];
        return true;
    }

    private void WriteMemoryDataInstruction(MemoryData memoryData, INode node, InstructionWriter writer)
    {
        if (node is DataNode<int> dataNode)
        {
            writer.InstructionList.Add((uint)dataNode.NodeData);
            return;
        }

        Register? memoryDataRegister = writer.GetIdentifierData(out _, memoryData.IdentifierId);
        OperatorCodes registerCode = memoryDataRegister is not null ?
            memoryDataRegister.RegisterCode : OperatorCodes.TM;

        writer.InstructionList.AddRange((uint)registerCode,
            (uint)memoryData.Size,
            (uint)memoryData.Offset);
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
        _ => OperatorCodes.Jump
    };
}
