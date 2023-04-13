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
        int lastRegisterOffset = LastRegisterData.Offset;
        AddRegisterData(registerMemoryList, lastRegisterOffset, registerData, (ulong)Math.Abs(value));
        AddRegisterData(offsetIndexList, lastRegisterOffset, registerData, (ulong)CalculateOffsetIndex(value));
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
        int offsetIndex = (int)GetRegisterValue(offsetIndexList.Span[registerIndex], currentOffset, 4);
        return (ulong)(dynamic)(returnData - returnData * 2 * offsetIndex);
    }

    /// <summary>
    /// This method will provide you an exact
    /// index of a <see cref="RegisterData"/> in memory.
    /// </summary>
    private int CalculateMemoryIndex(RegisterData registerData)
    {
        if (registerData.Offset < RegisterSize)
            return 0;
        if (registerData.Offset >= LastRegisterData.Offset)
            return registerMemoryList.Count - 1;

        int totalLastOffset = LastRegisterData.Offset + RegisterSize;
        int currentRegisterOffset = (totalLastOffset - registerData.Offset) / RegisterSize;

        return (registerMemoryList.Count - 1) - (currentRegisterOffset);
    }

    /// <summary>
    /// Add a value to the specified <see cref="NativeMemoryList{T}"/>
    /// <paramref name="registerMemory"/> that could potentially be shifted.
    /// </summary>
    private void AddRegisterData(NativeMemoryList<ulong> registerMemory, int lastOffset, RegisterData registerData, ulong value)
    {
        int offsetDifference = registerData.Offset - lastOffset;
        ulong currentValue = value << offsetDifference;

        if (registerMemory.Count == 0 || registerData.Size == RegisterSize || offsetDifference == RegisterSize
            || (registerData.Offset - lastOffset) > RegisterSize)
        {
            int registerOffsetIndex = offsetDifference / RegisterSize;
            registerOffsetIndex = registerOffsetIndex == 0 ? 1 : registerOffsetIndex;
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
        if (registerData.Size == RegisterSize)
            return 0;
        if (registerData.Offset < RegisterSize)
            return registerData.Offset;

        int relativeLastIndex = (RegisterSize * ((registerMemoryList.Count - 1) - registerIndex));
        return Math.Abs(Math.Abs(((LastRegisterData.Offset + RegisterSize) - (registerData.Offset + registerData.Size))) - relativeLastIndex);
    }

    /// <summary>
    /// Calculates index of a <paramref name="value"/>.
    /// </summary>
    /// <returns>
    /// If <see langword="int"/> <paramref name="value"/> is
    /// greater than 0, returns one, otherwise zero.
    /// </returns>
    private int CalculateOffsetIndex(int value) =>
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
