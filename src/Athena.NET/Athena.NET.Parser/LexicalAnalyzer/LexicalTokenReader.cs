using System.Text;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer
{
    internal abstract class LexicalTokenReader : StreamReader, IAsyncDisposable
    {
        private static readonly Encoding defaultEncoding =
            Encoding.UTF8;

        public long ReaderPosition { get; }

        public LexicalTokenReader(Stream stream) : base(stream, defaultEncoding, false)
        {
            ReaderPosition = stream.Position;
        } 
    }
}
