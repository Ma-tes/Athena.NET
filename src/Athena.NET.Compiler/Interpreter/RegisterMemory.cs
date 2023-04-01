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
            //TODO: Calculate relative offset for a new register value
            ulong finalValue = (ulong)value << (registerData.Offset - LastRegisterData.Offset);

            int totalMemorySize = registerData.Offset + registerData.Size;
            if (registerMemoryList.Count == 0 ||
                totalMemorySize >= RegisterSize * registerMemoryList.Count)
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

            int currentOffset = CalculateRelativeOffset(registerData.Offset, registerIndex);
            registerMemoryList.Span[registerIndex] =
                (registerMemoryList.Span[registerIndex] ^ (((ulong)value ^ ((registerMemoryList.Span[registerIndex] >> currentOffset)
                & (ulong)typeSize)) << currentOffset));
        }

        public ulong GetData(RegisterData registerData)
        {
            int registerIndex = CalculateMemoryIndex(registerData);
            ulong currentRegister = registerMemoryList.Span[registerIndex];
            int currentOffset = CalculateRelativeOffset(registerData.Offset, registerIndex);

            int typeSize = (int)Math.Pow(2, registerData.Size) - 1;
            int returnData = (int)((long)(currentRegister >> currentOffset)
                & typeSize & typeSize);
            return (ulong)(dynamic)returnData; //TODO: Make sure, to avoid the dynamic cast
        }

        private int CalculateMemoryIndex(RegisterData registerData) 
        {
            int totalMemorySize = registerData.Offset + registerData.Size;
            return totalMemorySize / (RegisterSize + 1);
        }

        private int CalculateRelativeOffset(int offset, int registerIndex)
        {
            int currentOffset = offset - registerIndex * RegisterSize;
            return (Math.Abs(currentOffset) + currentOffset) / 2;
        }

        public void Dispose()
        {
            registerMemoryList.Dispose();
        }
    }
}
