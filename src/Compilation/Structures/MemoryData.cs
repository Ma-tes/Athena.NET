namespace Athena.NET.Compilation.Structures;

internal readonly struct MemoryData
{
    public uint IdentifierId { get; }
    public int Offset { get; }
    public int Size { get; }

    public MemoryData(ReadOnlyMemory<char> identifierName, int offset, int size)
    {
        IdentifierId = CalculateIdentifierId(identifierName);
        Offset = offset;
        Size = size;
    }

    public static uint CalculateIdentifierId(ReadOnlyMemory<char> identifierName)
    {
        ReadOnlySpan<char> identifierSpan = identifierName.Span;
        uint resultIdentificator = 0;
        for (int i = 0; i < identifierSpan.Length; i++)
        {
            uint currentValue = ((byte)identifierSpan[i]) + (uint)i;
            resultIdentificator += currentValue;
        }
        return resultIdentificator;
    }
}
