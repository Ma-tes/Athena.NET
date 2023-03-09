namespace Athena.NET.Athena.NET.Compiler.DataHolders
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
    }
}
