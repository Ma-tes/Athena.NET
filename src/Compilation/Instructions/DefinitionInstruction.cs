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
        if (node.NodeToken != Lexing.TokenIndentificator.Unknown)
        {
            MemoryData definitionMemoryData = instructionWriter.TemporaryRegisterTM.AddRegisterData(leftDefinitionNode.DefinitionIdentifier.NodeData, 16);
            AddStoreInstruction(definitionMemoryData, instructionWriter);
        }
        uint definitionIdentificator = MemoryData.CalculateIdentifierId(leftDefinitionNode.DefinitionIdentifier.NodeData);
        ReadOnlyMemory<MemoryData> argumentsData = GetArgumentsMemoryData(leftDefinitionNode.NodeData, instructionWriter);
        instructionWriter.DefinitionList.Add(new DefinitionData<ReadOnlyMemory<MemoryData>>(
                definitionIdentificator, argumentsData
            ));
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
        var definitionData = new DefinitionData<int>(instructions[1], writer.LastInstructionNopIndex);
        writer.DefinitionList.Add(definitionData);
        return true;
    }

    private void CreateArgumentsInstructions(ReadOnlyMemory<MemoryData> argumentsData, InstructionWriter instructionWriter) 
    {
        int argumentsDataLength = argumentsData.Length;
        for (int i = 0; i < argumentsDataLength; i++) { AddStoreInstruction(argumentsData.Span[i], instructionWriter); }
    }

    private void AddStoreInstruction(MemoryData memoryData, InstructionWriter instructionWriter)
    {
        instructionWriter.InstructionList.AddRange((uint)OperatorCodes.Nop,
            (uint)OperatorCodes.Store);
        instructionWriter.AddMemoryDataInstructions(OperatorCodes.TM, memoryData);
        instructionWriter.InstructionList.Add(0);
    }

    private ReadOnlyMemory<MemoryData> GetArgumentsMemoryData(ReadOnlyMemory<InstanceNode> argumentInstances, InstructionWriter instructionWriter)
    {
        int instancesLength = argumentInstances.Length;
        if (instancesLength == 0)
            return null;

        ReadOnlySpan<InstanceNode> instancesSpan = argumentInstances.Span;
        Memory<MemoryData> returnRegisters = new MemoryData[instancesLength];
        for (int i = 0; i < instancesLength; i++)
        {
            ReadOnlyMemory<char> argumentIdentificator = instancesSpan[i].NodeData;
            MemoryData argumentMemoryData = instructionWriter.TemporaryRegisterTM.AddRegisterData(argumentIdentificator, 16);
            returnRegisters.Span[i] = argumentMemoryData;
        }
        return returnRegisters;
    }
} 
