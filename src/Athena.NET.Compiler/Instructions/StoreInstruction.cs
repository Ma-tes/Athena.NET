using Athena.NET.Parser;
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
                    return false;
            }
            return true;
        }
    }
}
