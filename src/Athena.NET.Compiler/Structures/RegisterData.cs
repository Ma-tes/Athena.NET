namespace Athena.NET.Compiler.Structures
{
    internal readonly struct RegisterData
    {
        public int Offset { get; }
        public int Size { get; }

        public RegisterData(uint offset, uint size)
        {
            Offset = (int)offset;
            Size = (int)size;
        }
    }
}
