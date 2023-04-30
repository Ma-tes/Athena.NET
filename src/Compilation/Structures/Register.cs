using Athena.NET.Compilation.DataHolders;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Athena.NET.Compilation.Structures;

/// <summary>
/// Is a specific memory manager, that can be created by single 
/// <see cref="RegisterCode"/> and <see cref="TypeSize"/>.
/// </summary>
internal sealed class Register : IDisposable
{
    internal NativeMemoryList<MemoryData> memoryData;
    private int lastOffset = 0;

    /// <summary>
    /// Specified maximum size of current register in bits.
    /// <code><see cref="Marshal.SizeOf{T}"/> * 8</code>
    /// </summary>
    public int TypeSize { get; }
    /// <summary>
    /// Register code from <see cref="OperatorCodes"/>.
    /// </summary>
    public OperatorCodes RegisterCode { get; }

    public Register(OperatorCodes registerCode, Type type)
    {
        RegisterCode = registerCode;
        TypeSize = Marshal.SizeOf(type) * 8;
        memoryData = new NativeMemoryList<MemoryData>();
    }

    /// <summary>
    /// Attach initialized <see cref="MemoryData"/> into
    /// <see cref="NativeMemoryList{T}"/> register.
    /// </summary>
    /// <param name="identificatorName">Name of an instance or an identifier.</param>
    /// <param name="dataSize">Size of data in a bits.</param>
    /// <returns>
    /// Corresponding <see cref="MemoryData"/>, that were already attached.
    /// </returns>
    public MemoryData AddRegisterData(ReadOnlyMemory<char> identificatorName, int dataSize)
    {
        var returnData = new MemoryData(identificatorName, lastOffset, dataSize);
        memoryData.Add(returnData);
        lastOffset += dataSize;

        return returnData;
    }

    /// <summary>
    /// If identificator exists, then it removes corresponding
    /// <see cref="MemoryData"/> by identifier id.
    /// </summary>
    /// <param name="identifierId">Identificator of a specific identifier.</param>
    public void RemoveRegisterData(uint identifierId)
    {
        if (TryGetIndexOfIdentifier(out int identifierIndex, identifierId))
            memoryData.RemoveOn(identifierIndex);
    }

    /// <summary>
    /// Tries to get a correspoding <see cref="MemoryData"/> by identifier id.
    /// </summary>
    /// <returns>
    /// Coresponding <see cref="bool"/>, whether <see cref="MemoryData"/> was found.
    /// </returns>
    public bool TryGetMemoryData([NotNullWhen(true)] out MemoryData resultData, uint identiferId)
    {
        if (TryGetIndexOfIdentifier(out int identifierIndex, identiferId))
        {
            resultData = memoryData.Span[identifierIndex];
            return true;
        }

        resultData = default;
        return false;
    }

    /// <summary>
    /// Tries to a get a value index from <see cref="NativeMemoryList{T}"/>
    /// <see cref="memoryData"/> by identifier id.
    /// </summary>
    /// <param name="returnIndex"><see langword="out"/> return value.</param>
    /// <param name="identifierId">Identificator of a specific identifier.</param>
    /// <returns>Coresponding <see cref="bool"/>, whether indetificator in
    /// <see cref="MemoryData"/> was found.</returns>
    private bool TryGetIndexOfIdentifier(out int returnIndex, uint identifierId)
    {
        Span<MemoryData> memoryDataSpan = memoryData.Span;
        for (int i = 0; i < memoryDataSpan.Length; i++)
        {
            if (identifierId == memoryDataSpan[i].IdentifierId)
            {
                returnIndex = i;
                return true;
            }
        }
        returnIndex = -1;
        return false;
    }

    /// <summary>
    /// Calculates the maximum offset for current data.
    /// </summary>
    /// <returns>
    /// If data offset isn't greater than <see cref="TypeSize"/>, returns offset as bit count,<br/>
    /// otherwise returns <see cref="TypeSize"/> as a maximum size.
    /// </returns>
    public int CalculateByteSize(int data)
    {
        int currentOffset = 0;
        while (currentOffset != TypeSize)
        {
            int offsetValue = (data >> (currentOffset)) & 255;
            if (offsetValue == 0)
                return currentOffset;
            currentOffset += 4;
        }
        return TypeSize;
    }

    /// <summary>
    /// Disposes and reinitializes its memory.
    /// </summary>
    public void ReDispose()
    {
        Dispose();
        memoryData = new NativeMemoryList<MemoryData>();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        memoryData.Dispose();
    }
}
