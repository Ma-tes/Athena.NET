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
        ReadOnlySpan<MemoryData> argumentsData = GetArgumentsMemoryData(leftDefinitionNode.NodeData, instructionWriter);
        CreateArgumentsInstructions(argumentsData, instructionWriter);

        instructionWriter.InstructionList.AddRange(
            (uint)OperatorCodes.Nop,
            (uint)OperatorCodes.Definition,
            definitionIdentificator);
        BodyNode rightBodyNode = (BodyNode)node.ChildNodes.RightNode;
        instructionWriter.CreateInstructions(rightBodyNode.NodeData.Span);
        return true;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
        var definitionData = new DefinitionData(instructions[1], writer.LastInstructionNopIndex);
        writer.DefinitionList.Add(definitionData);
        return true;
    }

    private void CreateArgumentsInstructions(ReadOnlySpan<MemoryData> memoryData, InstructionWriter instructionWriter) 
    {
        int memoryDataLength = memoryData.Length;
        for (int i = 0; i < memoryDataLength; i++)
        {
            instructionWriter.InstructionList.AddRange((uint)OperatorCodes.Nop,
                (uint)OperatorCodes.Store);
            MemoryData currentData = memoryData[i];
            instructionWriter.AddMemoryDataInstructions(OperatorCodes.TM, currentData);
            instructionWriter.InstructionList.Add(0);
        }
    }

    private ReadOnlySpan<MemoryData> GetArgumentsMemoryData(ReadOnlyMemory<InstanceNode> argumentInstances, InstructionWriter instructionWriter)
    {
        int instancesLength = argumentInstances.Length;
        if (instancesLength == 0)
            return null;

        ReadOnlySpan<InstanceNode> instancesSpan = argumentInstances.Span;
        Span<MemoryData> returnRegisters = new MemoryData[instancesLength];
        for (int i = 0; i < instancesLength; i++)
        {
            ReadOnlyMemory<char> argumentIdentificator = instancesSpan[i].NodeData;
            MemoryData argumentMemoryData = instructionWriter.TemporaryRegisterTM.AddRegisterData(argumentIdentificator, 16);
            returnRegisters[i] = argumentMemoryData;
        }
        return returnRegisters;
    }
} 
