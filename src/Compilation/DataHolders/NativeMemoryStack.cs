namespace Athena.NET.Compilation.DataHolders;

/// <summary>
/// Provides stack based memory management, for every <see langword="unmanaged"/> type,
/// which is then stored in <see cref="Span{T}"/>, with apropriete pointer(<see langword="void*"/>).
/// <br/>It's fully based on <see cref="NativeMemoryList{T}"/>.
/// </summary>
/// <typeparam name="T">Generic type, which needs to be <see cref="struct"/>.</typeparam>
public unsafe sealed class NativeMemoryStack<T> : NativeMemoryList<T>
    where T : struct
{
    /// <summary>
    /// Provide pushing <paramref name="data"/> on
    /// the 0 index, by shifting every other index.
    /// </summary>
    public void Push(T data)
    {
        if (Count == allocationLength)
            MemoryPointer = ReallocateMemory(4);
        memoryBuffer[..Count].CopyTo(memoryBuffer[1..]);
        memoryBuffer[0] = data;
        Count++;
    }

    /// <summary>
    /// Provide pushing <paramref name="data"/> on
    /// first indexes, by shifting every other index.
    /// </summary>
    public void PushRange(params T[] data)
    {
        Span<T> dataSpan = data.AsSpan();
        PushRange(dataSpan);
    }

    /// <summary>
    /// Provide pushing <paramref name="data"/> on
    /// first indexes, by shifting every other index.
    /// </summary>
    public void PushRange(Span<T> data)
    {
        int dataLength = data.Length;
        int countDifference = allocationLength - Count;
        if (dataLength > countDifference)
            MemoryPointer = ReallocateMemory(dataLength);

        memoryBuffer[..Count].CopyTo(memoryBuffer[dataLength..]);
        data.CopyTo(memoryBuffer[..dataLength]);
        Count += dataLength;
    }
}
