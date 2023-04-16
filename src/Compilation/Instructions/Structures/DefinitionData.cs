using Athena.NET.Compilation.Structures;

namespace Athena.NET.Compilation.Instructions.Structures;

public readonly struct DefinitionData 
{
    public uint Identificator { get; }
    public int DefinitionIndex { get; }

    public DefinitionData(uint identificator, int index)
    {
        Identificator = identificator;
        DefinitionIndex = index;
    }
} 
