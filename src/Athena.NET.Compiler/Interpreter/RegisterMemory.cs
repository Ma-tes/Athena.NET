using Athena.NET.Compiler.Structures;
using Athena.NET.Compiler.DataHolders;
using System.Runtime.InteropServices;

namespace Athena.NET.Compiler.Interpreter
{
    /// <summary>
    /// A custom register memory manager, that will provide storing
    /// specific values in a single <see langword="long"/>,
    /// that can be changed by <see cref="RegisterData"/>
    /// </summary>
    internal sealed class RegisterMemory : IDisposable
    {
        private readonly NativeMemoryList<ulong> registerMemoryList = new();
        private readonly NativeMemoryList<ulong> offsetIndexList = new();

        /// <summary>
        /// Register code of choosed register
        /// from a <see cref="OperatorCodes"/>
        /// </summary>
        public OperatorCodes RegisterCode { get; }
        /// <summary>
        /// Size of a one element in memory
        /// </summary>
        public int RegisterSize { get; }
        /// <summary>
        /// Last added <see cref="RegisterData"/> into
        /// a <see cref="NativeMemoryList{T}"/>
        /// </summary>
        public RegisterData LastRegisterData { get; private set; } =
            new(0, 0);

        public RegisterMemory(OperatorCodes registerCode, Type type)
        {
            RegisterCode = registerCode;
            RegisterSize = Marshal.SizeOf(type) * 8;
        }

        /// <summary>
        /// Provides adding an a <see langword="int"/> value with
        /// a coresponding <see cref="RegisterData"/>
        /// </summary>
        /// <remarks>
        /// This solutions is going to be
        /// fully generics
        /// </remarks>
        /// <param name="registerData">
        /// Valid <see cref="RegisterData"/> with specified
        /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>
        /// </param>
        /// <param name="value">Value that will be stored in a memory</param>
        public void AddData(RegisterData registerData, int value)
        {
            int absoluteValue = Math.Abs(value);

            int finalOffset = CalculateRelativeOffset(registerData, registerMemoryList.Count - 1);
            ulong finalValue = (ulong)absoluteValue << finalOffset;
            ulong offsetIndex = (ulong)(CalculateOffsetIndex(value) << finalOffset);

            int totalMemorySize = registerData.Offset + registerData.Size;
            if (totalMemorySize > (RegisterSize * registerMemoryList.Count))
            {
                registerMemoryList.Add(default);
                offsetIndexList.Add(default);
                finalValue = (ulong)absoluteValue;
                offsetIndex = (byte)CalculateOffsetIndex(value);
            }

            registerMemoryList.Span[registerMemoryList.Count - 1] += finalValue;
            offsetIndexList.Span[offsetIndexList.Count - 1] += offsetIndex;
            LastRegisterData = registerData;
        }

        /// <summary>
        /// Provides setting an a <see langword="int"/> value to
        /// a coresponding <see cref="RegisterData"/>
        /// </summary>
        /// <remarks>
        /// This solutions is going to be
        /// fully generics
        /// </remarks>
        /// <param name="registerData">
        /// Valid and already added <see cref="RegisterData"/> with specified
        /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>
        /// </param>
        /// <param name="value">Value that will be replaces in a memory</param>
        public void SetData(RegisterData registerData, int value)
        {
            int typeSize = (int)Math.Pow(2, registerData.Size) - 1;
            int registerIndex = CalculateMemoryIndex(registerData);

            int currentOffset = CalculateRelativeOffset(registerData, registerIndex);
            ulong offsetIndex = (ulong)(CalculateOffsetIndex(value) << currentOffset);
            offsetIndexList.Span[registerIndex] = offsetIndex;

            int absoluteValue = Math.Abs(value);
            ref ulong currentRegisterValue = ref registerMemoryList.Span[registerIndex];
            currentRegisterValue = currentRegisterValue ^ (((ulong)absoluteValue ^ ((currentRegisterValue >> currentOffset)
                & (ulong)typeSize)) << currentOffset);
        }

        /// <summary>
        /// Provides getting an a <see langword="ulong"/> value by
        /// a coresponding <see cref="RegisterData"/>
        /// </summary>
        /// <remarks>
        /// This solutions is going to be
        /// fully generics
        /// </remarks>
        /// <param name="registerData">
        /// Valid and already added <see cref="RegisterData"/> with specified
        /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>
        /// </param>
        public ulong GetData(RegisterData registerData)
        {
            int registerIndex = CalculateMemoryIndex(registerData);
            ulong currentRegister = registerMemoryList.Span[registerIndex];
            int currentOffset = CalculateRelativeOffset(registerData, registerIndex);

            int returnData = (int)GetRegisterValue(currentRegister, currentOffset, registerData.Size);
            int offsetIndex = (int)GetRegisterValue(offsetIndexList.Span[registerIndex], currentOffset, 4);

            return (ulong)(dynamic)(returnData - ((returnData * 2) * offsetIndex));
        }

        // This method will provide you an exact
        // index value of a RegisterData in a memory.
        private int CalculateMemoryIndex(RegisterData registerData) 
        {
            if (registerData.Offset == 0 || RegisterCode == OperatorCodes.TM)
                return registerData.Offset / RegisterSize;

            int totalMemorySize = registerData.Offset + registerData.Size;
            int currentOffsetSize = registerData.Offset > RegisterSize ? (registerData.Offset + registerData.Size) / RegisterSize : 1;
            return totalMemorySize / (RegisterSize + ((RegisterSize * (currentOffsetSize)) / registerData.Offset));
        }

        // This method will recalculate your offset
        // in a relative way to your RegisterSize
        private int CalculateRelativeOffset(RegisterData registerData, int registerIndex)
        {
            if (registerIndex == 0)
                return registerData.Offset;

            int offsetShift = (registerData.Offset / RegisterSize) >> 1;
            if (registerIndex < 0 || registerData.Offset >> offsetShift == RegisterSize)
                return 0;
            int relativeOffset = (registerData.Size / RegisterSize) ^ 1;
            return (registerData.Offset - ((RegisterSize * (registerIndex - relativeOffset)) + registerData.Size)) * relativeOffset;
        }

        private int CalculateOffsetIndex(int value) =>
           (((Math.Abs(value) + value) >> 1) / value) ^ 1;

        private ulong GetRegisterValue(ulong registerData, int offset, int size) =>
            (ulong)((long)(registerData >> offset) & ((int)Math.Pow(2, size) - 1));

        /// <summary>
        /// Manage dispose for a <see cref="NativeMemoryList{T}"/> 
        /// <see cref="registerMemoryList"/>
        /// </summary>
        public void Dispose()
        {
            registerMemoryList.Dispose();
        }
    }
}
