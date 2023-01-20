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

        //Actually I'am not happy with this
        //solution, I hope, that I will fix
        //this as soon as possible
        public bool TryGetKeyword([NotNullWhen(true)] out ReservedKeyword returnData, ReadOnlyMemory<char> source)
        {
            returnData = null!;
            if (IsEqual(source)) 
            {
                int keywordLength = KeywordData.Length;
                char nextSourceCharacter = source.Span[(keywordLength + 1)];

                if (KeywordsHolder.Character.IsEqual(nextSourceCharacter) ||
                    KeywordsHolder.Character.IsEqual(nextSourceCharacter)) 
                    return false;

                returnData = this;
                return true;
            }
            return false;
        }

        public bool IsEqual(ReadOnlyMemory<char> source) 
        {
            if (source.Length < KeywordData.Length)
                return false;

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
                new (TokenIndentificator.EqualLogical, "=="),

                new (TokenIndentificator.Whitespace, " "),
                new (TokenIndentificator.Semicolon, ";"),
                new (TokenIndentificator.Add, "+"),
                new (TokenIndentificator.Sub, "-"),
                new (TokenIndentificator.EqualAsigment, "=")
             };
    }
}
