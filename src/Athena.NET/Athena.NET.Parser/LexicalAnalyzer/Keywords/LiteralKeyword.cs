using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces;
using System.Diagnostics.CodeAnalysis;

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

    internal sealed class LiteralKeyword : IKeyword<LiteralOption, char, LiteralKeyword>
    {
        public TokenIndentificator Identificator { get; }
        public LiteralOption KeywordData { get; }

        public LiteralKeyword(TokenIndentificator id, LiteralOption data) 
        {
            Identificator = id;
            KeywordData = data;
        }

        public bool TryGetKeyword([NotNullWhen(true)]out LiteralKeyword returnData, char source) 
        {
            bool equalSource = IsEqual(source);
            returnData = equalSource ? this : null!;

            return equalSource;
        }

        public bool IsEqual(char source) =>
            (source >= KeywordData.Start && source <= KeywordData.End);
    }

    internal static partial class KeywordsHolder
    {
        public static readonly LiteralKeyword Digit =
            new(TokenIndentificator.Int, new LiteralOption('0', '9'));

        //This is here, just for testing
        public static readonly LiteralKeyword Character =
            new(TokenIndentificator.Char, new LiteralOption('a', 'z'));
    }
}
