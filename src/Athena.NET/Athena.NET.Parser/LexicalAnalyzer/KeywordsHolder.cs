using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords;
using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords.Literal;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer
{
    internal static class KeywordsHolder
    {
        public static LiteralKeyword Digit { get; } =
            new(TokenIndentificator.Int, new LiteralOption('0', '9'));

        //This is here, just for testing
        public static LiteralKeyword Character { get; } =
            new(TokenIndentificator.Char, new LiteralOption('a', 'z'));

        //TODO: I would really like to have a
        //better storing system for overall keywords
        public static ReadOnlyMemory<ReservedKeyword> ReservedKeywords =
            new ReservedKeyword[]
            {
                new (TokenIndentificator.Int, "int"),
                new (TokenIndentificator.Char, "char"),
                new (TokenIndentificator.If, "if"),
                new (TokenIndentificator.EqualLogical, "=="),

                //I know this implementation is actually
                //horrible, but for now is somehow acceptable
                new (TokenIndentificator.EndLine, "\0n", true)
                {
                    ParseFunction = (ReadOnlyMemory<char> data) =>
                         (data.ToString()
                              .Replace("\r\n", "\0n")
                              .ToCharArray())
                },
                new (TokenIndentificator.Tabulator, "\t", true),
                new (TokenIndentificator.Whitespace, " ", true),
                new (TokenIndentificator.Semicolon, ";", true),
                new (TokenIndentificator.Add, "+", true),
                new (TokenIndentificator.Sub, "-", true),
                new (TokenIndentificator.EqualAsigment, "=", true),
                new (TokenIndentificator.OpenBrace, "(", true),
                new (TokenIndentificator.CloseBrace, ")", true)
             };
    }
}
