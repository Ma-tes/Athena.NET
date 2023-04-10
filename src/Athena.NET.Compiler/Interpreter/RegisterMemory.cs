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
        /// This solutions is going to be fully generics
        /// </remarks>
        /// <param name="registerData">
        /// Valid <see cref="RegisterData"/> with specified
        /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>
        /// </param>
        /// <param name="value">Value that will be stored in a memory</param>
        public void AddData(RegisterData registerData, int value)
        { 
            int memoryIndex = CalculateMemoryIndex(registerData);
            int finalOffset = CalculateRelativeOffset(registerData, memoryIndex);
            int totalMemorySize = registerData.Offset + registerData.Size;

            AddRegisterData(registerMemoryList, totalMemorySize, finalOffset, (ulong)Math.Abs(value));
            AddRegisterData(offsetIndexList, totalMemorySize, finalOffset, (ulong)CalculateOffsetIndex(value));
            LastRegisterData = new RegisterData((uint)finalOffset, (uint)registerData.Size);
        }

        /// <summary>
        /// Provides setting an a <see langword="int"/> <paramref name="value"/> to
        /// a coresponding <see cref="RegisterData"/>
        /// </summary>
        /// <remarks>
        /// This solutions is going to be fully generics
        /// </remarks>
        /// <param name="registerData">
        /// Valid and already added <see cref="RegisterData"/> with specified
        /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>
        /// </param>
        /// <param name="value">Value that will be replaces in a memory</param>
        public void SetData(RegisterData registerData, int value)
        {
            int registerIndex = CalculateMemoryIndex(registerData);
            int currentOffset = CalculateRelativeOffset(registerData, registerIndex);

            registerMemoryList.Span[registerIndex] = SetRegisterData(registerMemoryList.Span[registerIndex], registerData.Size, currentOffset, Math.Abs(value));
            offsetIndexList.Span[registerIndex] = SetRegisterData(offsetIndexList.Span[registerIndex], 4, currentOffset, CalculateOffsetIndex(value));
        }

        /// <summary>
        /// Provides getting an a <see langword="ulong"/> value by
        /// a coresponding <see cref="RegisterData"/>
        /// </summary>
        /// <remarks>
        /// This solutions is going to be fully generics
        /// </remarks>
        /// <param name="registerData">
        /// Valid and already added <see cref="RegisterData"/> with specified
        /// <see cref="RegisterData.Offset"/> and <see cref="RegisterData.Size"/>
        /// </param>
        public ulong GetData(RegisterData registerData)
        {
            int registerIndex = CalculateMemoryIndex(registerData);
            int currentOffset = CalculateRelativeOffset(registerData, registerIndex);

            registerIndex = registerIndex >= registerMemoryList.Count ? registerIndex - (registerMemoryList.Count - 1) : registerIndex;
            int returnData = (int)GetRegisterValue(registerMemoryList.Span[registerIndex], currentOffset, registerData.Size);
            int offsetIndex = (int)GetRegisterValue(offsetIndexList.Span[registerIndex], currentOffset, 4);
            return (ulong)(dynamic)(returnData - ((returnData * 2) * offsetIndex));
        }

        /// <summary>
        /// This method will provide you an exact
        /// index value of a <see cref="RegisterData"/> in a memory.
        /// </summary>
        private int CalculateMemoryIndex(RegisterData registerData) 
        {
            //TODO: Try to avoid this expensive if statement
            if (registerData.Offset == 0 || registerData.Size == RegisterSize || RegisterCode == OperatorCodes.TM)
            {
                int returnMemoryIndex = ((registerData.Offset + (registerData.Size / 2)) / RegisterSize);
                return returnMemoryIndex > registerMemoryList.Count - 1 ? registerMemoryList.Count - 1 : returnMemoryIndex;
            }
            int registerDifference = (registerData.Offset - RegisterSize);
            int sizeIndex = ((Math.Abs(registerDifference) + registerDifference) >> 1) / (registerDifference - 1);
            int offsetIndex = ((registerData.Offset / RegisterSize) - ((registerData.Offset - registerData.Size) / RegisterSize)) ^ 1;

            return (registerData.Offset - (registerData.Size * sizeIndex * offsetIndex)) / RegisterSize;
        }

        /// <summary>
        /// Add a value to a specified, <see cref="NativeMemoryList{T}"/>
        /// <paramref name="registerMemory"/>, that could be potentially shifted
        /// </summary>
        private void AddRegisterData(NativeMemoryList<ulong> registerMemory, int totalMemorySize, int offset, ulong value)
        {
            ulong resultValue = value << offset;
            if (registerMemory.Count == 0 || ((LastRegisterData.Offset + LastRegisterData.Size) / (RegisterSize)) == 1)
            {
                registerMemory.Add(default);
                resultValue = value;
            }
            int lastIndex = registerMemory.Count - 1;
            registerMemory.Span[lastIndex] += resultValue;
        }

        /// <summary>
        /// Set a new <paramref name="registerData"/> by shifting <see langword="int"/>
        /// <paramref name="value"/> with coresponding <paramref name="size"/> mask
        /// </summary>
        private ulong SetRegisterData(ulong registerData, int size, int offset, int value) =>
            registerData ^ (((ulong)value ^ ((registerData >> offset)
                & (ulong)((int)Math.Pow(2, size) - 1))) << offset);

        /// <summary>
        /// Recalculates your <see cref="RegisterData.Offset"/>
        /// in a relative way to your <see cref="RegisterData.Size"/>
        /// </summary>
        private int CalculateRelativeOffset(RegisterData registerData, int registerIndex)
        {
            if (registerIndex == 0)
                return registerData.Offset;

            if (registerIndex < 0 || (RegisterSize * (registerIndex)) >= registerData.Offset)
                return 0;
            int relativeOffset = (registerData.Size / RegisterSize) ^ 1;
            return ((registerData.Offset - ((RegisterSize * (registerIndex - relativeOffset)) + registerData.Size)) * relativeOffset) / (RegisterSize / registerData.Size);
        }

        /// <summary>
        /// Calculates index of a <paramref name="value"/>
        /// </summary>
        /// <returns>
        /// If <see langword="int"/> <paramref name="value"/> is
        /// greater then 0, it will returns one, otherwise zero
        /// </returns>
        private int CalculateOffsetIndex(int value) =>
           (((Math.Abs(value) + value) >> 1) / value) ^ 1; 

        /// <summary>
        /// Provides calculation of original value from <paramref name="registerData"/>,
        /// that is shifted by <paramref name="offset"/> and reduced by
        /// calculation of mask from <paramref name="size"/>
        /// </summary>
        private ulong GetRegisterValue(ulong registerData, int offset, int size) =>
            (ulong)((long)(registerData >> offset) & ((int)Math.Pow(2, size) - 1));

        /// <summary>
        /// Manage dispose for all <see cref="NativeMemoryList{T}"/> such as,
        /// <br/><see cref="registerMemoryList"/>
        /// <br/><see cref="offsetIndexList"/>
        /// </summary>
        public void Dispose()
        {
            registerMemoryList.Dispose();
            offsetIndexList.Dispose();
        }
    }
}
