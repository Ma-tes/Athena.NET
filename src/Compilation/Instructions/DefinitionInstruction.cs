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

        if (!instructionWriter.TryGetDefinitionData(out DefinitionData currentDefinitionData,
            MemoryData.CalculateIdentifierId(leftDefinitionNode.DefinitionIdentifier.NodeData)))
            return false;
        ReadOnlyMemory<MemoryData> currentArgumentsData = currentDefinitionData.DefinitionArguments;
        CreateArgumentsInstructions(currentArgumentsData, instructionWriter);

        BodyNode rightBodyNode = (BodyNode)node.ChildNodes.RightNode;
        instructionWriter.CreateInstructions(rightBodyNode.NodeData.Span);
        return true;
    }

    public bool InterpretInstruction(ReadOnlySpan<uint> instructions, VirtualMachine writer)
    {
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
} 
