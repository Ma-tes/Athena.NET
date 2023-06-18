using Athena.NET.Compilation.Interpretation;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.Parsing.Nodes.Statements;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Compilation.Instructions;

internal sealed class StoreInstruction : IInstruction<EqualAssignStatement>
{
    public bool EmitInstruction(EqualAssignStatement node, InstructionWriter writer)
    {
        ChildrenNodes childrenNodes = node.ChildNodes;
        if (childrenNodes.RightNode is OperatorNode operatorNode)
        {
            if (TryEmitDataNodeChildrens(out Register? returnRegister, operatorNode, writer)
                && returnRegister is not null)
                return true;

            var operatorInstruction = new OperatorInstruction();
            if (!operatorInstruction.EmitInstruction(operatorNode, writer))
                return false;
            writer.InstructionList.Add((uint)OperatorCodes.Nop);
            Register currentRegister = writer.GetEmitIntRegister(
                operatorInstruction.EmitMemoryData.Size)!;

            bool instructionResult = TryWriteStoreInstruction(childrenNodes.LeftNode, currentRegister, currentRegister.TypeSize, writer);
            writer.AddMemoryDataInstructions(OperatorCodes.TM, operatorInstruction.EmitMemoryData);
            return instructionResult;
        }

        writer.InstructionList.Add((uint)OperatorCodes.Nop);
        if (childrenNodes.RightNode is IdentifierNode identifierNode)
        {
            Register? identifierRegister = writer.GetIdentifierData(out MemoryData rightData, identifierNode.NodeData);
            if (identifierRegister is null)
                return false;

            bool instructionResult = TryWriteStoreInstruction(childrenNodes.LeftNode, identifierRegister, rightData.Size, writer);
            writer.AddMemoryDataInstructions(identifierRegister.RegisterCode, rightData);
            return instructionResult;
        }

        int nodeData = ((DataNode<int>)childrenNodes.RightNode).NodeData;
        Register? emitRegister = writer.GetEmitIntRegister(nodeData)!;
        bool writeInstructions = TryWriteStoreInstruction(childrenNodes.LeftNode, emitRegister,
            emitRegister.CalculateByteSize(nodeData), writer);
        writer.InstructionList.Add((uint)nodeData);
        return writeInstructions;
    }

    //TODO: Implement more cohesive way
    //of tokenizing instructions
    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
        int currentData = writer.GetInstructionData(instructions[4..])[0];
        var storeRegister = new RegisterData(instructions[3], instructions[2]);
        if (writer.TryGetRegisterMemory(out RegisterMemory? storeMemory, (OperatorCodes)instructions[1]))
        {
            RegisterData lastRegisterData = storeMemory.LastRegisterData;
            if (lastRegisterData.Offset < storeRegister.Offset
                || (lastRegisterData.Offset + lastRegisterData.Size) == 0)
                storeMemory.AddData(storeRegister, currentData);
            else
                storeMemory.SetData(storeRegister, currentData);
            return true;
        }
        return false;
    }

    private bool TryEmitDataNodeChildrens([NotNullWhen(true)] out Register returnRegister, OperatorNode operatorNode,
        InstructionWriter writer)
    {
        ChildrenNodes childNodes = operatorNode.ChildNodes;
        if (childNodes.LeftNode is DataNode<int> leftData &&
            childNodes.RightNode is DataNode<int> rightData)
        {
            int operatorData = operatorNode.CalculateData(leftData.NodeData, rightData.NodeData);
            returnRegister = writer.GetEmitIntRegister(operatorData)!;
            return TryWriteStoreInstruction(childNodes.LeftNode, returnRegister,
                returnRegister!.CalculateByteSize(operatorData), writer);
        }
        return NullableHelper.NullableOutValue(out returnRegister);
    }


    private bool TryWriteStoreInstruction(INode dataNode, Register register, int size, InstructionWriter writer)
    {
        MemoryData currentMemoryData = default;
        if (dataNode is IdentifierNode identifierNode)
        {
            Register? identiferRegister = writer.GetIdentifierData(out currentMemoryData,
                identifierNode.NodeData);
            if (identiferRegister is null)
                return false;

            if (currentMemoryData.Size != size)
            {
                register.RemoveRegisterData(currentMemoryData.IdentifierId);
                currentMemoryData = register.AddRegisterData(identifierNode.NodeData, size);
            }
        }
        if (dataNode is InstanceNode instanceNode)
            currentMemoryData = register.AddRegisterData(instanceNode.NodeData, size);

        writer.InstructionList.Add((uint)OperatorCodes.Store);
        writer.AddMemoryDataInstructions(register.RegisterCode, currentMemoryData);
        return true;
    }
}
