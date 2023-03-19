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
            int currentCount = registerMemoryList.Count;

            if (currentCount == 0 ||
                totalMemorySize > RegisterSize)
                registerMemoryList.Add(default);

            registerMemoryList.Span[currentCount - 1] += (ulong)(value >> registerData.Offset);
        }

        public void Dispose() 
        {
            registerMemoryList.Dispose();
        }
    }
}
