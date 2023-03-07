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
            writer.InstructionList.Add((uint)OperatorCodes.Store);
            ChildrenNodes childrenNodes = node.ChildNodes;
            if(childrenNodes.RightNode is DataNode<int> dataNode)
                return TryWriteStoreInstruction(childrenNodes.LeftNode, dataNode.NodeData, writer);
            if (childrenNodes.RightNode is OperatorNode operatorNode) 
            {
                if (operatorNode.ChildNodes.LeftNode is DataNode<int> leftData &&
                    operatorNode.ChildNodes.RightNode is DataNode<int> rightData) 
                {
                    int operatorData = operatorNode.CalculateData(leftData.NodeData, rightData.NodeData);
                    return TryWriteStoreInstruction(childrenNodes.LeftNode, operatorData, writer);
                }
            }
            return true;
        }

        private bool TryWriteStoreInstruction(INode dataNode, int data, InstructionWriter writer)
        {
            Register dataRegister = writer.GetEmitIntRegister(data)!;
            if (dataRegister is null)
                return false;
            if (dataNode is InstanceNode instanceNode) 
            {
                var memoryData = dataRegister.AddRegisterData(instanceNode.NodeData, data);
                writer.InstructionList.Add((uint)memoryData.Size);
                writer.InstructionList.Add((uint)memoryData.Offset);
                writer.InstructionList.Add((uint)dataRegister.RegisterCode);
                writer.InstructionList.Add((uint)data);
                return true;
            }
            //TODO: Create an implementation of
            //the identifierNode
            return false;
        }
    }
}
