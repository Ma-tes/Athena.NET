using Athena.NET.Compilation.Structures;
using Athena.NET.Compilation.DataHolders;
using System.Runtime.InteropServices;

namespace Athena.NET.Compilation.Interpretation;

/// <summary>
/// A custom register memory manager, that will provide storing
/// specific values in a single <see langword="long"/>,
/// that can be changed by <see cref="RegisterData"/>.
/// </summary>
internal sealed class RegisterMemory : IDisposable
{
    private readonly NativeMemoryList<ulong> registerMemoryList = new();
    private readonly NativeMemoryList<ulong> offsetIndexList = new();

    /// <summary>
    /// Register code of choosed register
    /// from a <see cref="OperatorCodes"/>.
    /// </summary>
    public OperatorCodes RegisterCode { get; }
    /// <summary>
    /// Size of a single element in memory.
    /// </summary>
    public int RegisterSize { get; }
    /// <summary>
    /// Last added <see cref="RegisterData"/> into
    /// a <see cref="NativeMemoryList{T}"/>.
    /// </summary>
    public RegisterData LastRegisterData { get; private set; } =
        new(0, 0);
    public RegisterData previousRegisterData { get; private set; } =
        new(0, 0);

    public RegisterMemory(OperatorCodes registerCode, Type type)
    {
        RegisterCode = registerCode;
        RegisterSize = Marshal.SizeOf(type) * 8;
    }

    //TODO: Make generic
    /// <summary>
    /// Provides adding an <see langword="int"/> value with
    /// a coresponding <see cref="RegisterData"/>.
    /// </summary>
    /// <param name="registerData">
    /// Valid <see cref="RegisterData"/> with specified
    /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>.
    /// </param>
    /// <param name="value">Value that will be stored in a memory.</param>
    public void AddData(RegisterData registerData, int value)
    {
        int lastRegisterIndex = registerMemoryList.Count;
        AddRegisterData(registerMemoryList, LastRegisterData.Offset, registerData, (ulong)Math.Abs(value));
        if(registerMemoryList.Count > lastRegisterIndex)
            AddRegisterData(offsetIndexList, 0, new((uint)registerData.Offset, 0), (ulong)registerData.Offset);

        AddRegisterData(offsetIndexList, LastRegisterData.Offset, new((uint)registerData.Offset, 0), (ulong)CalculateOffsetIndex(value) << RegisterSize);
        previousRegisterData = registerData;
    }

    //TODO: Make generic
    /// <summary>
    /// Provides setting an a <see langword="int"/> <paramref name="value"/> to
    /// a corresponding <see cref="RegisterData"/>.
    /// </summary>
    /// <param name="registerData">
    /// Valid and already added <see cref="RegisterData"/> with specified
    /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>.
    /// </param>
    /// <param name="value">Value that will be replaced in memory.</param>
    public void SetData(RegisterData registerData, int value)
    {
        int registerIndex = CalculateMemoryIndex(registerData);
        int currentOffset = CalculateRelativeOffset(registerData, registerIndex);
        registerIndex = registerIndex >= registerMemoryList.Count ? registerIndex - (registerMemoryList.Count - 1) : registerIndex;

        registerMemoryList.Span[registerIndex] = SetRegisterData(registerMemoryList.Span[registerIndex], registerData.Size, currentOffset, Math.Abs(value));
        offsetIndexList.Span[registerIndex] = SetRegisterData(offsetIndexList.Span[registerIndex], 4, currentOffset, CalculateOffsetIndex(value));
    }

    //TODO: Make generic
    /// <summary>
    /// Provides getting an a <see langword="ulong"/> value by
    /// a corresponding <see cref="RegisterData"/>.
    /// </summary>
    /// <param name="registerData">
    /// Valid and already added <see cref="RegisterData"/> with specified
    /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>.
    /// </param>
    public ulong GetData(RegisterData registerData)
    {
        int registerIndex = CalculateMemoryIndex(registerData);
        int currentOffset = CalculateRelativeOffset(registerData, registerIndex);

        int returnData = (int)GetRegisterValue(registerMemoryList.Span[registerIndex], currentOffset, registerData.Size);
        int offsetIndex = (int)GetRegisterValue(offsetIndexList.Span[registerIndex], currentOffset + RegisterSize, 4);
        return (ulong)(dynamic)(returnData - returnData * 2 * offsetIndex);
    }

    //TODO: Try to avoid using for loop statement
    /// <summary>
    /// This method will provide you an exact
    /// index of a <see cref="RegisterData"/> in memory.
    /// </summary>
    private int CalculateMemoryIndex(RegisterData registerData)
    {
        int offsetRegisterCount = offsetIndexList.Count;
        for (int i = 0; i < offsetRegisterCount; i++)
        {
            ulong currentRegisterIndex = offsetIndexList.Span[i];
            int currentRelativeOffset = (int)GetRegisterValue(currentRegisterIndex, 0, RegisterSize);

            int registerDifference = registerData.Offset - currentRelativeOffset;
            if (registerDifference == 0 || registerDifference < RegisterSize)
                return i;
        }
        return offsetIndexList.Count - 1;
    }

    /// <summary>
    /// Add a value to the specified <see cref="NativeMemoryList{T}"/>
    /// <paramref name="registerMemory"/> that could potentially be shifted.
    /// </summary>
    private void AddRegisterData(NativeMemoryList<ulong> registerMemory, int lastOffset, RegisterData registerData, ulong value)
    {
        int offsetDifference = registerData.Offset - lastOffset;
        ulong currentValue = value << offsetDifference;

        if (registerMemory.Count == 0 || registerData.Size == RegisterSize
            || offsetDifference >= RegisterSize)
        {
            int previousOffsetDifference = registerData.Offset - previousRegisterData.Offset;
            int registerOffsetIndex = previousOffsetDifference <= RegisterSize ? 1 : (previousOffsetDifference / RegisterSize) + 1;
            for (int i = 0; i < registerOffsetIndex; i++) { registerMemory.Add(default); } //TODO: Find a better solution

            currentValue = value;
            LastRegisterData = registerData;
        }
        registerMemory.Span[registerMemory.Count - 1] += currentValue;
    }

    /// <summary>
    /// Set a new <paramref name="registerData"/> by shifting <see langword="int"/>
    /// <paramref name="value"/> with corresponding <paramref name="size"/> mask.
    /// </summary>
    private ulong SetRegisterData(ulong registerData, int size, int offset, int value) =>
        registerData ^ ((ulong)value ^ registerData >> offset
            & (ulong)((int)Math.Pow(2, size) - 1)) << offset;

    /// <summary>
    /// Recalculates your <see cref="RegisterData.Offset"/>
    /// in a relative way to your <see cref="RegisterData.Size"/>.
    /// </summary>
    private int CalculateRelativeOffset(RegisterData registerData, int registerIndex)
    {
        ulong currentOffsetIndex = offsetIndexList.Span[registerIndex];
        int currentOffset = (int)GetRegisterValue(currentOffsetIndex, 0, RegisterSize);
        return registerData.Offset - currentOffset;
    }

    /// <summary>
    /// Calculates index of a <paramref name="value"/>.
    /// </summary>
    /// <returns>
    /// If <see langword="int"/> <paramref name="value"/> is
    /// greater than 0, returns one, otherwise zero.
    /// </returns>
    private int CalculateOffsetIndex(int value) => value == 0 ? 0 :
       (Math.Abs(value) + value >> 1) / value ^ 1;

    /// <summary>
    /// Calculates original value from <paramref name="registerData"/>,
    /// that is shifted by <paramref name="offset"/> and reduced by
    /// calculation of mask from <paramref name="size"/>.
    /// </summary>
    private ulong GetRegisterValue(ulong registerData, int offset, int size) =>
        (ulong)((long)(registerData >> offset) & (int)Math.Pow(2, size) - 1);

    /// <inheritdoc/>
    public void Dispose()
    {
        registerMemoryList.Dispose();
        offsetIndexList.Dispose();
    }
}
