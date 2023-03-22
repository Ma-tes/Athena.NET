using Athena.NET.Athena.NET.Compiler.Structures;
using Athena.NET.Compiler;
using Athena.NET.Compiler.DataHolders;
using System.Runtime.InteropServices;

namespace Athena.NET.Athena.NET.Compiler.DataHolders
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
            if (registerMemoryList.Count == 0 ||
                totalMemorySize > RegisterSize)
                registerMemoryList.Add(default);

            registerMemoryList.Span[registerMemoryList.Count - 1] += (ulong)(value << registerData.Offset);
        }

        public T GetData(RegisterData registerData) 
        {
            int totalMemorySize = registerData.Offset + registerData.Size;
            int registerIndex = totalMemorySize / RegisterSize;

            ulong currentRegister = registerMemoryList.Span[registerIndex];
            int currentOffset = totalMemorySize - (registerIndex * RegisterSize);

            int typeSize = (int)Math.Pow(2, registerData.Size) - 1;
            var returnData = (int)((long)(currentRegister >> currentOffset) & (typeSize) & (typeSize));
            return (T)(dynamic)returnData; //TODO: Make sure, to avoid the dynamic cast
        }

        public void Dispose() 
        {
            registerMemoryList.Dispose();
        }
    }
}
