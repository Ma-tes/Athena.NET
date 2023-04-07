namespace Athena.NET.Compiler.DataHolders
{
    public unsafe sealed class NativeMemoryStack<T> : NativeMemoryList<T>
        where T : struct
    {
        public void Push(T data) 
        {
            if (Count == allocationLength)
                MemoryPointer = ReallocateMemory(4);
            memoryBuffer[..Count].CopyTo(memoryBuffer[1..]);
            memoryBuffer[0] = data;
            Count++;
        }

        public void PushRange(params T[] data) 
        {
            Span<T> dataSpan = data.AsSpan();
            PushRange(dataSpan);
        }

        public void PushRange(Span<T> data) 
        {
            int dataLength = data.Length;
            int countDifference = allocationLength - Count;
            if (dataLength > countDifference) 
                MemoryPointer = ReallocateMemory(dataLength);

            memoryBuffer[..Count].CopyTo(memoryBuffer[dataLength..]);
            data.CopyTo(memoryBuffer[..dataLength]);
            Count += dataLength;
        } 
    }
}
