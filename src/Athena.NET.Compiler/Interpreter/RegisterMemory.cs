using Athena.NET.Athena.NET.Compiler.Structures;
using Athena.NET.Compiler;
using Athena.NET.Compiler.DataHolders;
using System.Runtime.InteropServices;

namespace Athena.NET.Athena.NET.Compiler.Interpreter
{
    internal sealed class RegisterMemory<T> : IDisposable
        where T : unmanaged
    {
        private readonly NativeMemoryList<ulong> registerMemoryList
            = new();

        public OperatorCodes RegisterCode { get; }
        public int RegisterSize { get; }

        public RegisterMemory(OperatorCodes registerCode)
        {
            RegisterCode = registerCode;
            RegisterSize = Marshal.SizeOf<T>() * 8;
        }

        public RegisterMemory(OperatorCodes registerCode, int allocationSize)
        {
            RegisterCode = registerCode;
            RegisterSize = allocationSize;
        }

        public void AddData(RegisterData registerData, int value)
        {
            int totalMemorySize = registerData.Offset + registerData.Size;

            ulong finalValue = (ulong)value << registerData.Offset;
            if (registerMemoryList.Count == 0 ||
                totalMemorySize > RegisterSize)
            {
                registerMemoryList.Add(default);
                finalValue = (ulong)value;
            }
            registerMemoryList.Span[registerMemoryList.Count - 1] += finalValue;
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

        public T GetData(RegisterData registerData)
        {
            int registerIndex = CalculateMemoryIndex(registerData);
            ulong currentRegister = registerMemoryList.Span[registerIndex];
            int currentOffset = CalculateRelativeOffset(registerData.Offset, registerIndex);

            int typeSize = (int)Math.Pow(2, registerData.Size) - 1;
            int returnData = (int)((long)(currentRegister >> currentOffset)
                & typeSize & typeSize);
            return (T)(dynamic)returnData; //TODO: Make sure, to avoid the dynamic cast
        }

        private int CalculateMemoryIndex(RegisterData registerData) 
        {
            int totalMemorySize = registerData.Offset + registerData.Size;
            return totalMemorySize / RegisterSize;
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
