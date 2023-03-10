using Athena.NET.Compiler.DataHolders;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Athena.NET.Compiler.Structures
{
    internal sealed class Register : IDisposable
    {
        private NativeMemoryList<MemoryData> memoryData;
        private int lastOffset = 0;

        public int TypeSize { get; }
        public OperatorCodes RegisterCode { get; }

        public Register(OperatorCodes registerCode, Type type)
        {
            RegisterCode = registerCode;
            TypeSize = Marshal.SizeOf(type) * 8;
            memoryData = new NativeMemoryList<MemoryData>();
        }

        //TODO: Make sure, that data value
        //is need to be an a generic unmanged
        //type, maybe consider implementing
        //a INumber interface
        public MemoryData AddRegisterData(ReadOnlyMemory<char> identificatorName, int dataSize)
        {
            var returnData = new MemoryData(identificatorName, lastOffset, dataSize);
            memoryData.Add(returnData);
            lastOffset += dataSize;

            return returnData;
        }

        public bool TryGetMemoryData([NotNullWhen(true)]out MemoryData resultData, uint identiferId)
        {
            var memoryDataSpan = memoryData.Span;
            for (int i = 0; i < memoryDataSpan.Length; i++)
            {
                MemoryData currentData = memoryDataSpan[i];
                if (identiferId == currentData.IdentifierId)
                {
                    resultData = currentData;
                    return true;
                }
            }
            resultData = default;
            return false;
        }

        public bool TryGetMemoryData([NotNullWhen(true)]out MemoryData resultData, ReadOnlyMemory<char> identifierName)
        {
            uint identiferId = MemoryData.CalculateIdentifierId(identifierName);
            return TryGetMemoryData(out resultData, identiferId);
        }

        public int CalculateByteSize(int data)
        {
            int currentOffset = 0;
            while (currentOffset != TypeSize)
            {
                int offsetValue = (data >> (currentOffset)) & 255;
                if (offsetValue == 0)
                    return currentOffset;
                currentOffset += 4;
            }
            return TypeSize;
        }

        public void ReDispose()
        {
            Dispose();
            memoryData = new NativeMemoryList<MemoryData>();
        }

        public void Dispose()
        {
            memoryData.Dispose();
        }
    }
}
