using Athena.NET.Compilation.Structures;

namespace Athena.NET.Compilation.Instructions.Structures;

public struct DefinitionInformation
{
    public int LastCallIndex { get; set; } = 0;
    public ReadOnlyMemory<MemoryData> DefinitionArguments { get; }

    public DefinitionInformation(ReadOnlyMemory<MemoryData> definitionArguments) 
    {
        DefinitionArguments = definitionArguments;
    }
}
