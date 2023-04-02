using Athena.NET.Compiler.Structures;
using Athena.NET.Compiler.DataHolders;
using System.Runtime.InteropServices;

namespace Athena.NET.Compiler.Interpreter
{
    internal sealed class RegisterMemory : IDisposable
    {
        private readonly NativeMemoryList<ulong> registerMemoryList
            = new();

        public OperatorCodes RegisterCode { get; }
        public int RegisterSize { get; }

        public RegisterData LastRegisterData { get; private set; } =
            new(0, 0);

        public RegisterMemory(OperatorCodes registerCode, Type type)
        {
            RegisterCode = registerCode;
            RegisterSize = Marshal.SizeOf(type) * 8;
        }

        public void AddData(RegisterData registerData, int value)
        {
            int finalOffset = CalculateRelativeOffset(registerData, registerMemoryList.Count - 1);
            ulong finalValue = (ulong)value << finalOffset;

            int totalMemorySize = registerData.Offset + registerData.Size;
            if (registerMemoryList.Count == 0 || totalMemorySize > RegisterSize * registerMemoryList.Count)
            {
                registerMemoryList.Add(default);
                finalValue = (ulong)value;
            }
            registerMemoryList.Span[registerMemoryList.Count - 1] += finalValue;
            LastRegisterData = registerData;
        }

        public void SetData(RegisterData registerData, int value)
        {
            int typeSize = (int)Math.Pow(2, registerData.Size) - 1;
            int registerIndex = CalculateMemoryIndex(registerData);

            int currentOffset = CalculateRelativeOffset(registerData, registerIndex);
            registerMemoryList.Span[registerIndex] =
                (registerMemoryList.Span[registerIndex] ^ (((ulong)value ^ ((registerMemoryList.Span[registerIndex] >> currentOffset)
                & (ulong)typeSize)) << currentOffset));
        }

        public ulong GetData(RegisterData registerData)
        {
            int registerIndex = CalculateMemoryIndex(registerData);
            ulong currentRegister = registerMemoryList.Span[registerIndex];
            int currentOffset = CalculateRelativeOffset(registerData, registerIndex);

            int typeSize = (int)Math.Pow(2, registerData.Size) - 1;
            int returnData = (int)((long)(currentRegister >> currentOffset)
                & typeSize & typeSize);
            return (ulong)(dynamic)returnData; //TODO: Make sure, to avoid the dynamic cast
        }

        private int CalculateMemoryIndex(RegisterData registerData) 
        {
            if (registerData.Offset == 0 || RegisterCode == OperatorCodes.TM)
                return registerData.Offset / RegisterSize;

            int totalMemorySize = registerData.Offset + registerData.Size;
            return totalMemorySize / (RegisterSize + (RegisterSize / registerData.Offset));
        }

        private int CalculateRelativeOffset(RegisterData registerData, int registerIndex)
        {
            if (registerIndex == 0)
                return registerData.Offset;
            if (registerIndex < 0 || registerData.Offset % RegisterSize == 0)
                return 0;
            int relativeOffset = (registerData.Size / RegisterSize) ^ 1;
            return (registerData.Offset - ((RegisterSize * (registerIndex - relativeOffset)) + registerData.Size)) * relativeOffset;
        }

        public void Dispose()
        {
            registerMemoryList.Dispose();
        }
    }
}
