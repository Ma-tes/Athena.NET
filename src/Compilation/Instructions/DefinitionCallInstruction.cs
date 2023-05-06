using Athena.NET.Compilation.Instructions.Structures;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.Parsing.Nodes.Statements;

namespace Athena.NET.Compilation.Instructions;

internal sealed class DefinitionCallInstruction : IInstruction<CallStatement>
{
    public bool EmitInstruction(CallStatement node, InstructionWriter writer) 
    {
        IdentifierNode leftIdentifierNode = (IdentifierNode)node.ChildNodes.LeftNode;
        DefinitionCallNode rightCallNode = (DefinitionCallNode)node.ChildNodes.RightNode;

        uint definitionIdentificator = MemoryData.CalculateIdentifierId(leftIdentifierNode.NodeData);
        if (!writer.TryGetDefinitionData(out DefinitionData currentDefinitionData, definitionIdentificator) ||
            rightCallNode.NodeData.Length != currentDefinitionData.DefinitionArguments.Length)
            return false;

        ReadOnlyMemory<MemoryData> definitionArgumentsData = currentDefinitionData.DefinitionArguments;
        ReadOnlySpan<INode> argumentNodes = rightCallNode.NodeData.Span;
        for (int i = 0; i < argumentNodes.Length; i++)
        {
            MemoryData currentArgumentMemoryData = definitionArgumentsData.Span[i];
            if (!CreateArgumentStoreInstructions(argumentNodes[i], currentArgumentMemoryData, writer))
                return false;
        }

        int currentJumpIndex = currentDefinitionData.DefinitionIndex - (writer.InstructionList.Count + 3);
        if (!writer.TemporaryRegisterTM.TryGetMemoryData(out MemoryData definitionData, definitionIdentificator))
            definitionData = writer.TemporaryRegisterTM.AddRegisterData(leftIdentifierNode.NodeData, 16);
        AddJumpStoreInstruction(definitionData, ((currentJumpIndex + (currentDefinitionData.DefinitionLength)) * -1), writer);

        writer.InstructionList.AddRange((uint)OperatorCodes.Nop,
            (uint)OperatorCodes.Jump,
            (uint)currentJumpIndex);
        return true;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
        //TODO: Create a better use case for
        //interpretation of this instruction
        return false;
    }

    private bool CreateArgumentStoreInstructions(INode callArgumentNode, MemoryData definitionArgumentData,
        InstructionWriter writer)
    {
        writer.InstructionList.AddRange((uint)OperatorCodes.Nop, (uint)OperatorCodes.Store);
        writer.AddMemoryDataInstructions(OperatorCodes.TM, definitionArgumentData);
        if (callArgumentNode is OperatorNode operatorNode)
        {
            var operatorInstruction = new OperatorInstruction();
            if (!operatorInstruction.EmitInstruction(operatorNode, writer))
                return false;
            writer.AddMemoryDataInstructions(OperatorCodes.TM, operatorInstruction.EmitMemoryData);
            return true;
        }

        if (callArgumentNode is IdentifierNode identifierNode)
        {
            Register? idetifierRegister = writer.GetIdentifierData(out MemoryData identifierData, identifierNode.NodeData);
            if (idetifierRegister is null)
                return false;
            writer.AddMemoryDataInstructions(idetifierRegister.RegisterCode, identifierData);
            return true;
        }
        writer.InstructionList.Add((uint)((DataNode<int>)callArgumentNode).NodeData);
        return true;
    }

    private void AddJumpStoreInstruction(MemoryData memoryData, int jumpIndex, InstructionWriter instructionWriter)
    {
        instructionWriter.InstructionList.AddRange((uint)OperatorCodes.Nop,
            (uint)OperatorCodes.Store);
        instructionWriter.AddMemoryDataInstructions(OperatorCodes.TM, memoryData);
        instructionWriter.InstructionList.Add((uint)jumpIndex);
    }
}
