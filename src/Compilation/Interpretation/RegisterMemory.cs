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
        int memoryIndex = CalculateMemoryIndex(registerData);
        int finalOffset = CalculateRelativeOffset(registerData, memoryIndex);
        int totalMemorySize = registerData.Offset + registerData.Size;

        AddRegisterData(registerMemoryList, totalMemorySize, finalOffset, (ulong)Math.Abs(value));
        AddRegisterData(offsetIndexList, totalMemorySize, finalOffset, (ulong)CalculateOffsetIndex(value));
        LastRegisterData = new RegisterData((uint)finalOffset, (uint)registerData.Size);
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

        registerIndex = registerIndex >= registerMemoryList.Count ? registerIndex - (registerMemoryList.Count - 1) : registerIndex;
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
        //TODO: Try to avoid this expensive if statement
        if (registerData.Offset == 0 || registerData.Size == RegisterSize || RegisterCode == OperatorCodes.TM)
        {
            int returnMemoryIndex = (registerData.Offset + registerData.Size / 2) / RegisterSize;
            return returnMemoryIndex > registerMemoryList.Count - 1 ? registerMemoryList.Count - 1 : returnMemoryIndex;
        }
        int registerDifference = registerData.Offset - RegisterSize;
        int sizeIndex = (Math.Abs(registerDifference) + registerDifference >> 1) / (registerDifference - 1);
        int offsetIndex = registerData.Offset / RegisterSize - (registerData.Offset - registerData.Size) / RegisterSize ^ 1;

        return (registerData.Offset - registerData.Size * sizeIndex * offsetIndex) / RegisterSize;
    }

    /// <summary>
    /// Add a value to the specified <see cref="NativeMemoryList{T}"/>
    /// <paramref name="registerMemory"/> that could potentially be shifted.
    /// </summary>
    private void AddRegisterData(NativeMemoryList<ulong> registerMemory, int totalMemorySize, int offset, ulong value)
    {
        ulong resultValue = value << offset;
        if (registerMemory.Count == 0 || (LastRegisterData.Offset + LastRegisterData.Size) / RegisterSize == 1)
        {
            registerMemory.Add(default);
            resultValue = value;
        }
        int lastIndex = registerMemory.Count - 1;
        registerMemory.Span[lastIndex] += resultValue;
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
        if (registerIndex == 0)
            return registerData.Offset;

        if (registerIndex < 0 || RegisterSize * registerIndex >= registerData.Offset)
            return 0;
        int relativeOffset = registerData.Size / RegisterSize ^ 1;
        return (registerData.Offset - (RegisterSize * (registerIndex - relativeOffset) + registerData.Size)) * relativeOffset / (RegisterSize / registerData.Size);
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
