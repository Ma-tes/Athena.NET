namespace Athena.NET.Compiler.Structures.KeywordStructure
{
    public abstract class Keyword
    {
        protected abstract byte identificatorShift { get; }

        public ReadOnlyMemory<byte> KeywordData { get; }
        public int KeywordIndetificator { get; }

        public Keyword(int id, string name)
        {
            KeywordData = GetBytes(name);
            KeywordIndetificator = id << KeywordIndetificator;
        }

        private ReadOnlyMemory<byte> GetBytes(string text) 
        {
            int length = text.Length;
            Memory<byte> memoryHolder = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byte currentByte = (byte)text[i];
                memoryHolder.Span[i] = currentByte;
            }
            return memoryHolder;
        }
    }
}
