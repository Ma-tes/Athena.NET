namespace Athena.NET.Compilation.Instructions.Structures;

public readonly struct DefinitionData<T>
{
    public uint Identificator { get; }
    public T Data { get; }

    public DefinitionData(uint identificator, T data)
    {
        Identificator = identificator;
        Data = data;
    }
} 
