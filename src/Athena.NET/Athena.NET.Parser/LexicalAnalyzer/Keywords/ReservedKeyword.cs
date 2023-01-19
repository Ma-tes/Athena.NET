using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords
{
    internal sealed class ReservedKeyword : IKeyword<ReadOnlyMemory<char>, ReadOnlyMemory<char>, ReservedKeyword>
    {
        public TokenIndentificator Identificator { get; }
        public ReadOnlyMemory<char> KeywordData { get; }

        public ReservedKeyword(TokenIndentificator id, string data) 
        {
            Identificator = id;
            KeywordData = data.ToCharArray();
        }

        //TODO: Implement getting a keyword by checking, if the spans, with same size,
        //are equal to each other and then if there is not any literar character
        public bool TryGetKeyword([NotNullWhen(true)] out ReservedKeyword returnData, ReadOnlyMemory<char> source)
        {
            returnData = null!;
            return false;
        }

        public bool IsEqual(ReadOnlyMemory<char> source) 
        {
            int keywordLength = KeywordData.Length;
            var currentData = KeywordData[0..keywordLength];

            return currentData.Span == source.Span;
        }
    }

    internal static partial class KeywordsHolder
    {
        //TODO: I would really like to have a
        //better storing system for overall keywords
        public static ReadOnlyMemory<ReservedKeyword> ReservedKeywords =
            new ReservedKeyword[]
            { 
                new (TokenIndentificator.Int, "int"),
                new (TokenIndentificator.Char, "char"),
                new (TokenIndentificator.IF, "if"),
                new (TokenIndentificator.Whitespace, " "),
                new (TokenIndentificator.Semicolon, ";"),
                new (TokenIndentificator.Add, "+"),
                new (TokenIndentificator.Sub, "-"),
                new (TokenIndentificator.EqualAsigment, "="),
                new (TokenIndentificator.EqualLogical, "==")
             };
    }
}
