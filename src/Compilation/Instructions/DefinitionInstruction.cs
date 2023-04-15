using Athena.NET.Compilation.Instructions.Structures;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Statements.Body;

namespace Athena.NET.Compilation.Instructions;

internal sealed class DefinitionInstruction : IInstruction<DefinitionStatement>
{
    public bool EmitInstruction(DefinitionStatement node, InstructionWriter instructionWriter) 
    {
        DefinitionNode leftDefinitionNode = (DefinitionNode)node.ChildNodes.LeftNode;
        uint definitionIdentificator = MemoryData.CalculateIdentifierId(leftDefinitionNode.DefinitionIdentifier.NodeData);
        var definitionData = new DefinitionData(
                definitionIdentificator,
                instructionWriter.InstructionList.Count,
                CreateArgumentIndetificators(leftDefinitionNode.NodeData, definitionIdentificator)
            );

        instructionWriter.DefinitionList.Add(definitionData);
        instructionWriter.InstructionList.AddRange(
            (uint)OperatorCodes.Nop,
            (uint)OperatorCodes.Definition,
            definitionIdentificator, 0);
        int currentInstructionLength = instructionWriter.InstructionList.Count;
        BodyNode rightBodyNode = (BodyNode)node.ChildNodes.RightNode;

        instructionWriter.CreateInstructions(rightBodyNode.NodeData.Span);
        instructionWriter.InstructionList.Span[currentInstructionLength - 1] = (uint)(instructionWriter.InstructionList.Count - currentInstructionLength);
        return true;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer) 
    {
        return true;
    }

    private ReadOnlyMemory<uint> CreateArgumentIndetificators(ReadOnlyMemory<InstanceNode> argumentInstances, uint definitionIdentificator)
    {
        int instancesLength = argumentInstances.Length;
        if (instancesLength == 0)
            return null;

        Memory<uint> returnRegisters = new uint[instancesLength];
        for (int i = 0; i < instancesLength; i++)
        {
            uint currentIdentificator = MemoryData.CalculateIdentifierId(argumentInstances.Span[i].NodeData) + definitionIdentificator;
            returnRegisters.Span[i] = currentIdentificator;
        }
        return returnRegisters;
    }
} 
