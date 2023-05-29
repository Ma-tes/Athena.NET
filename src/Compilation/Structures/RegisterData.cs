using Athena.NET.Compilation.Interpretation;

namespace Athena.NET.Compilation.Structures;

/// <summary>
/// Provides structurized handeling of a
/// <see cref="Offset"/>, <see cref="Size"/>.
/// </summary>
public readonly struct RegisterData
{
    /// <summary>
    /// Calculated offset by already stored data
    /// in <see cref="RegisterMemory"/>.
    /// </summary>
    public int Offset { get; }

    /// <summary> 
    /// Determinated size by stored value.
    /// </summary>
    public int Size { get; }

    public RegisterData(uint offset, uint size)
    {
        Offset = (int)offset;
        Size = (int)size;
    }
}
