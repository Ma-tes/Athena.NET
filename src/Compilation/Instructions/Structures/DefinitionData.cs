namespace Athena.NET.Compilation.Instructions.Structures;

public readonly struct DefinitionData 
{
    public uint Identificator { get; }
    public int DefinitionIndex { get; }

    public ReadOnlyMemory<uint>? ArgumentIdentificators { get; }

    public DefinitionData(uint identificator, int index, ReadOnlyMemory<uint>? argumentIdentificators)
    {
        Identificator = identificator;
        DefinitionIndex = index;
        ArgumentIdentificators = argumentIdentificators;
    }
} 
