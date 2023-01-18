using Athena.NET.Athena.NET.Parser.Structures;
using System.Text;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer
{
    internal abstract class LexicalTokenReader : IDisposable
    {
        private static readonly string blankLine = "\0";
        private static readonly Encoding defaultEncoding =
            Encoding.UTF8;

        private StreamReader streamReader;

        public Memory<char> ReaderData { get; }

        public long ReaderLength { get; }
        public long ReaderPosition { get; private set; }

        public LexicalTokenReader(Stream stream)
        {
            streamReader = new(stream, defaultEncoding, false);

            ReaderLength = streamReader.BaseStream.Length;
            ReaderData = new char[ReaderLength];
        }

        public async Task<ReadOnlyMemory<Token>> ReadLexicalTokensAsync() 
        {
            await streamReader.ReadAsync(ReaderData);
            while (ReaderPosition != ReaderLength) 
            {
            }
        }

        protected abstract Token GetToken(ReadOnlyMemory<char> data);

        public void Dispose() 
        {
            streamReader.Dispose();
        }
    }
}
