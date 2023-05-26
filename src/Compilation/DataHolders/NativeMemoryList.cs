using System.Runtime.InteropServices;

namespace Athena.NET.Compilation.DataHolders;

/// <summary>
/// Provides memory management, for every <see langword="unmanaged"/> type, 
/// which is then stored in <see cref="Span{T}"/>, with apropriete pointer(<see langword="void*"/>).
/// </summary>
/// <typeparam name="T">Generic type, which needs to be <see cref="struct"/>.</typeparam>
public unsafe class NativeMemoryList<T> : IDisposable where T : struct //TODO: Implement an ICollection<T> interface
{
    /// <summary>
    /// Unmanaged <see cref="Span{T}"/>, that's created from
    /// <see cref="MemoryPointer"/> and
    /// sized by <see cref="allocationLength"/>.
    /// </summary>
    protected Span<T> memoryBuffer => new Span<T>(MemoryPointer, allocationLength);
    private nuint memoryAlignment;
    private int dataSize;

    /// <summary>
    /// Added size of every reallocation, where
    /// <see langword="default"/> value is 4.
    /// </summary>
    protected int allocationLength { get; private set; }

    /// <summary>
    /// Provides <see cref="Span{T}"/>, from <see cref="memoryBuffer"/>,
    /// which is reallocated by current allocation <see cref="Count"/>.
    /// </summary>
    public Span<T> Span => memoryBuffer[..Count];

    /// <summary>
    /// Pointer to specified memory buffer, provided
    /// by <see cref="NativeMemory.AlignedAlloc(nuint, nuint)"/>.
    /// </summary>
    public void* MemoryPointer { get; internal set; }

    /// <summary>
    /// Provides count of every added values.
    /// </summary>
    public int Count { get; protected set; }

    public NativeMemoryList(int allocationLength = 4)
    {
        dataSize = Marshal.SizeOf<T>();

        this.allocationLength = allocationLength;
        memoryAlignment = (nuint)Math.Pow(2, dataSize);
        MemoryPointer = NativeMemory.AlignedAlloc((nuint)(allocationLength * dataSize), memoryAlignment);
    }

    /// <summary>
    /// Adds and creates reference of <paramref name="data"/>, to
    /// <see cref="memoryBuffer"/> on <see cref="Count"/> index.
    /// </summary>
    public void Add(T data)
    {
        if (Count == allocationLength)
            MemoryPointer = ReallocateMemory(4);
        memoryBuffer[Count] = data;
        Count++;
    }

    /// <summary>
    /// Adds and creates reference of <paramref name="data"/>, to
    /// <see cref="memoryBuffer"/> on <see cref="Count"/> index.
    /// </summary>
    public void AddRange(ReadOnlySpan<T> data)
    {
        int dataLength = data.Length;
        int countDifference = allocationLength - Count;
        if (dataLength > countDifference)
            MemoryPointer = ReallocateMemory(dataLength - countDifference);

        data.CopyTo(memoryBuffer[Count..]);
        Count += dataLength;
    }

    //TODO: Create Span<T> params in .NET 8
    /// <summary>
    /// Adds and creates reference of <paramref name="data"/>, to
    /// <see cref="memoryBuffer"/> on <see cref="Count"/> index.
    /// </summary>
    public void AddRange(params T[] data)
    {
        Span<T> dataSpan = data.AsSpan();
        AddRange(dataSpan);
    }

    /// <summary>
    /// Removes value on specified <paramref name="index"/>,
    /// by providing copy on shifted indexes.
    /// </summary>
    /// <param name="index">Index of value in <see cref="Span"/>.</param>
    public void RemoveOn(int index)
    {
        memoryBuffer[(index + 1)..].CopyTo(memoryBuffer[index..]);
        ReallocateMemory(-1);
        Count--;
    }

    /// <summary>
    /// Creates a reallocation of current <see cref="allocationLength"/> and
    /// <paramref name="size"/>, by <see cref="NativeMemory.AlignedAlloc(nuint, nuint)"/>.
    /// </summary>
    /// <param name="size">Size of allocation for new indexes.</param>
    /// <returns>Pointer(<see langword="void*"/>) of new <see cref="MemoryPointer"/>.</returns>
    protected void* ReallocateMemory(int size)
    {
        allocationLength = allocationLength + size;
        return NativeMemory.AlignedRealloc(MemoryPointer, (nuint)(allocationLength * dataSize), memoryAlignment);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        NativeMemory.AlignedFree(MemoryPointer);
    }
}
