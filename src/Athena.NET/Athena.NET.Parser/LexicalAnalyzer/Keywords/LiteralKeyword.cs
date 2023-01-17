using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords
{
    internal readonly struct LiteralOption 
    {
        public byte Start { get; }
        public byte End { get; }

        public LiteralOption(char start, char end) 
        {
            Start = (byte)start;
            End = (byte)end;
        }
    }

    internal sealed class LiteralKeyword : IKeyword<LiteralOption, byte>
    {
        public int Identificator { get; }
        public LiteralOption KeywordData { get; }

        public LiteralKeyword(int id, LiteralOption data) 
        {
            Identificator = id;
            KeywordData = data;
        }

        public bool IsEqual(byte source) =>
            (source >= KeywordData.Start && source <= KeywordData.End);
    }

    internal static partial class KeywordsHolder
    {
        public static readonly LiteralKeyword Digit =
            new(0, new LiteralOption('0', '9'));

        //This is here, just for testing
        public static readonly LiteralKeyword Character =
            new(1, new LiteralOption('a', 'z'));
    }
}
