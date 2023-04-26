using Athena.NET.Compilation.Structures;

namespace Athena.NET.Compilation.Instructions.Structures;

public readonly struct DefinitionData
{
    public uint Identificator { get; }
    public int DefinitionIndex { get; }
    public ReadOnlyMemory<MemoryData> DefinitionArguments { get; }

    public DefinitionData(uint identificator, int definitionIndex, ReadOnlyMemory<MemoryData> definitionArguments)
    {
        Identificator = identificator;
        DefinitionIndex = definitionIndex;
        DefinitionArguments = definitionArguments;
    }
} 
