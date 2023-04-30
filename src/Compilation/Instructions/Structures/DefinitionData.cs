using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Nodes.Data;

namespace Athena.NET.Compilation.Instructions.Structures;

public struct DefinitionData
{
    public uint Identificator { get; }
    public int DefinitionIndex { get; set; }
    public int DefinitionLength { get; set; }

    public ReadOnlyMemory<MemoryData> DefinitionArguments { get; }
    public BodyNode DefinitionBodyNode { get; }

    public DefinitionData(uint identificator, int definitionIndex, int definitionLength,
        ReadOnlyMemory<MemoryData> definitionArguments, BodyNode definitionBodyNode)
    {
        Identificator = identificator;
        DefinitionIndex = definitionIndex;
        DefinitionLength = definitionLength;
        DefinitionArguments = definitionArguments;
        DefinitionBodyNode = definitionBodyNode;
    }
} 
