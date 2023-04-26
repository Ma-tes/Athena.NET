using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Interpretation;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Lexing;
using Athena.NET.Parsing;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;

namespace Athena.NET.Compilation.Instructions;

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

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
        TokenIndentificator operatorInstruction = (TokenIndentificator)((instructions[0] ^ (0xffee << 8)) - 2);
        if (!OperatorHelper.TryGetOperator(out OperatorNode instructionNode, operatorInstruction))
            return false;

        ReadOnlySpan<int> registerData = writer.GetInstructionData(instructions[4..]);
        int temporaryData = instructionNode.CalculateData(registerData[0], registerData[1]);
        if (writer.TryGetRegisterMemory(out RegisterMemory? registerMemory, OperatorCodes.TM))
        {
            var temporaryRegisterData = new RegisterData(instructions[3], instructions[2]);
            registerMemory.AddData(temporaryRegisterData, temporaryData);
        }
        return true;
    }

    private bool TryGenerateOperatorInstructions(out MemoryData returnData, INode node,
        InstructionWriter writer)
    {
        ChildrenNodes childrenNodes = node.ChildNodes;
        if (node is OperatorNode operatorNode)
        {
            OperatorCodes instructionOperator = GetOperatorNodeCode(operatorNode);
            var currentInstructions = new NativeMemoryStack<uint>();

            WriteMemoryDataInstructions(currentInstructions, childrenNodes.LeftNode, writer);
            WriteMemoryDataInstructions(currentInstructions, childrenNodes.RightNode, writer);

            returnData = writer.TemporaryRegisterTM.AddRegisterData(new char[1], 16);
            currentInstructions.PushRange((uint)OperatorCodes.Nop,
                (uint)instructionOperator,
                (uint)OperatorCodes.TM,
                (uint)returnData.Size,
                (uint)returnData.Offset);

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

    public void WriteMemoryDataInstructions(NativeMemoryList<uint> nativeInstructions, INode node,
        InstructionWriter writer)
    {
        if (TryGenerateOperatorInstructions(out MemoryData returnData, node, writer))
        {
            OperatorCodes registerCode = node is IdentifierNode identifierNode ?
                writer.GetIdentifierData(out _, identifierNode.NodeData)!.RegisterCode :
                OperatorCodes.TM;
            nativeInstructions.AddRange((uint)registerCode,
                (uint)returnData.Size,
                (uint)returnData.Offset);
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
