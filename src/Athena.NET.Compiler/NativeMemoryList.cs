using System.Runtime.InteropServices;

namespace Athena.NET.Athena.NET.Compiler
{
    public unsafe sealed class NativeMemoryList<T> : IDisposable where T : struct //TODO: Implement an ICollection<T> interface
    {
        private Span<T> memoryBuffer => new Span<T>(memoryPointer, allocationLength);
        private void* memoryPointer;
        private nuint memoryAlignment;

        private int dataSize;
        private int allocationLength;

        public Span<T> Span => memoryBuffer[..Count];
        public int Count { get; private set; }

        public NativeMemoryList(int allocationLength = 4)
        {
            dataSize = Marshal.SizeOf<T>();

            this.allocationLength = allocationLength;
            memoryAlignment = (nuint)Math.Pow(2, dataSize);
            memoryPointer = NativeMemory.AlignedAlloc((nuint)(allocationLength * dataSize), memoryAlignment);
        }

        public void Add(T data)
        {
            if (Count == allocationLength)
            {
                allocationLength = allocationLength + 4;
                memoryPointer = NativeMemory.AlignedRealloc(memoryPointer, (nuint)(allocationLength * dataSize), memoryAlignment);
            }
            memoryBuffer[Count] = data;
            Count++;
        }

        public void Dispose()
        {
            NativeMemory.AlignedFree(memoryPointer);
        }
    }
}
