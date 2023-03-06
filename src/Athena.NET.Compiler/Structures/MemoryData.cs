namespace Athena.NET.Athena.NET.Compiler.Structures
{
    internal readonly struct MemoryData
    {
        public ReadOnlyMemory<char> IdentifierName { get; }
        public int Offset { get; }
        public int Size { get; }

        public MemoryData(ReadOnlyMemory<char> identifierName, int offset, int size) 
        {
            IdentifierName = identifierName;
            Offset = offset;
            Size = size;
        }
    }
}
