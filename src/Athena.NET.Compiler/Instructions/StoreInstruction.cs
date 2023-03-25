using Athena.NET.Compiler.Interpreter;
using Athena.NET.Compiler.Structures;
using Athena.NET.Parser;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes;
using Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Compiler.Instructions
{
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
                    operatorInstruction.EmitMemoryData.Size * 2)!;

                bool instructionResult = TryWriteStoreInstruction(childrenNodes.LeftNode, currentRegister, currentRegister.TypeSize, writer);
                AddMemoryDataInstructions(OperatorCodes.TM, operatorInstruction.EmitMemoryData, writer);
                return instructionResult;
            }

            if (childrenNodes.RightNode is IdentifierNode identifierNode) 
            {
                Register? identifierRegister = writer.GetIdentifierData(out MemoryData rightData, identifierNode.NodeData);
                if (identifierRegister is null)
                    return false;

                bool instructionResult =  TryWriteStoreInstruction(childrenNodes.LeftNode, identifierRegister, rightData.Size, writer);
                AddMemoryDataInstructions(identifierRegister.RegisterCode, rightData, writer);
                return instructionResult;
            }

            int nodeData = ((DataNode<int>)childrenNodes.RightNode).NodeData;
            Register? emitRegister = writer.GetEmitIntRegister(nodeData)!;
            bool writeInstructions = TryWriteStoreInstruction(childrenNodes.LeftNode, emitRegister,
                emitRegister.CalculateByteSize(nodeData), writer);
            writer.InstructionList.Add((uint)nodeData);
            return writeInstructions;
        }

        public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer) 
        {
            return true;
        }

        private bool TryEmitDataNodeChildrens([NotNullWhen(true)]out Register returnRegister, OperatorNode operatorNode,
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

            returnRegister = null!;
            return false;
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
                    currentMemoryData = register.AddRegisterData(identifierNode.NodeData, size);
            }
            if (dataNode is InstanceNode instanceNode) 
                currentMemoryData = register.AddRegisterData(instanceNode.NodeData, size);

            writer.InstructionList.Add((uint)OperatorCodes.Store);
            AddMemoryDataInstructions(register.RegisterCode, currentMemoryData, writer);
            return true;
        }

        private void AddMemoryDataInstructions(OperatorCodes registerCode, MemoryData memoryData, InstructionWriter writer)
        {
            writer.InstructionList.AddRange((uint)registerCode,
                (uint)memoryData.Size,
                (uint)memoryData.Offset);
        }
    }
}
