using Athena.NET.Compilation.Structures;

namespace Athena.NET.Compilation.Instructions.Structures;

public readonly struct DefinitionData
{
    public uint Identificator { get; }
    public int DefinitionIndex { get; }
    public int DefinitionLength { get; }
    public ReadOnlyMemory<MemoryData> DefinitionArguments { get; }

    public DefinitionData(uint identificator, int definitionIndex, int definitionLength, ReadOnlyMemory<MemoryData> definitionArguments)
    {
        Identificator = identificator;
        DefinitionIndex = definitionIndex;
        DefinitionLength = definitionLength;
        DefinitionArguments = definitionArguments;
    }
} 
