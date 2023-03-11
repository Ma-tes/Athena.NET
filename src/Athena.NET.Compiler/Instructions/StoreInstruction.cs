using Athena.NET.Compiler.Structures;
using Athena.NET.Parser;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes;
using Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes;

namespace Athena.NET.Compiler.Instructions
{
    //TODO: Create complete implmentation
    //of this store instruction
    internal sealed class StoreInstruction : IInstruction<EqualAssignStatement>
    {
        public bool EmitInstruction(EqualAssignStatement node, InstructionWriter writer)
        {
            ChildrenNodes childrenNodes = node.ChildNodes;
            if (childrenNodes.RightNode is OperatorNode operatorNode)
            {
                if (operatorNode.ChildNodes.LeftNode is DataNode<int> leftData &&
                    operatorNode.ChildNodes.RightNode is DataNode<int> rightData)
                {
                    int operatorData = operatorNode.CalculateData(leftData.NodeData, rightData.NodeData);
                    Register? operatorRegister = writer.GetEmitIntRegister(operatorData)!;
                    return TryWriteStoreInstruction(childrenNodes.LeftNode, operatorRegister,
                        operatorRegister.CalculateByteSize(operatorData), writer);
                }
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
            writer.InstructionList.AddRange((uint)memoryData.Size,
                (uint)memoryData.Offset,
                (uint)registerCode);
        }
    }
}
