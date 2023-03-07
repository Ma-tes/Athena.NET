using Athena.NET.Athena.NET.Compiler.Structures;
using Athena.NET.Parser;
using Athena.NET.Parser.Interfaces;
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
                    return TryWriteStoreInstruction(childrenNodes.LeftNode, writer.GetEmitIntRegister(operatorData)!, operatorData, writer);
                }
                var operatorInstruction = new OperatorInstruction();
                if (!operatorInstruction.EmitInstruction(operatorNode, writer))
                    return false;

                writer.InstructionList.Add((uint)OperatorCodes.Nop);
                Register currentRegister = writer.GetEmitIntRegister(
                    operatorInstruction.EmitMemoryData.Size * 2)!;
                return TryWriteStoreInstruction(childrenNodes.LeftNode, currentRegister, (int)OperatorCodes.TM, writer);
            }

            int rightNodeData = ((DataNode<int>)childrenNodes.RightNode).NodeData;
            return TryWriteStoreInstruction(childrenNodes.LeftNode, writer.GetEmitIntRegister(rightNodeData)!, rightNodeData, writer);
        }

        private bool TryWriteStoreInstruction(INode dataNode, Register register, int data, InstructionWriter writer)
        {
            writer.InstructionList.Add((uint)OperatorCodes.Store);
            if (dataNode is InstanceNode instanceNode) 
            {
                var memoryData = register.AddRegisterData(instanceNode.NodeData, data);
                writer.InstructionList.Add((uint)memoryData.Size);
                writer.InstructionList.Add((uint)memoryData.Offset);
                writer.InstructionList.Add((uint)register.RegisterCode);
                writer.InstructionList.Add((uint)data);
                return true;
            }
            //TODO: Create an implementation of
            //the identifierNode
            return false;
        }
    }
}
