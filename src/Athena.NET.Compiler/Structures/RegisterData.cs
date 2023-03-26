namespace Athena.NET.Athena.NET.Compiler.Structures
{
    internal readonly struct RegisterData
    {
        public int Offset { get; }
        public int Size { get; }

        public RegisterData(int offset, int size) 
        {
            Offset = offset;
            Size = size;
        }
        public RegisterData(uint offset, uint size) 
        {
            Offset = (int)offset;
            Size = (int)size;
        }
    }
}
