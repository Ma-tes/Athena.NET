namespace Athena.NET.Compilation.Structures;

/// <summary>
/// Provides structurized handeling of a
/// <see cref="Offset"/>, <see cref="Size"/>.
/// </summary>
public readonly struct MemoryData
{
    /// <summary>
    /// Identificator related to <see cref="Offset"/>
    /// and <see cref="Size"/>.
    /// </summary>
    public uint IdentifierId { get; }

    /// <summary>
    /// Calculated offset by already stored data
    /// in <see cref="Register"/>.
    /// </summary>
    public int Offset { get; }

    /// <summary> 
    /// Determinated size by stored value.
    /// </summary>
    public int Size { get; }

    public MemoryData(ReadOnlyMemory<char> identifierName, int offset, int size)
    {
        IdentifierId = CalculateIdentifierId(identifierName);
        Offset = offset;
        Size = size;
    }

    /// <summary>
    /// Creates identificator, that is calculated by current
    /// <see cref="byte"/> and relative index.
    /// </summary>
    /// <returns>Calculated sum of every interation.</returns>
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
