using Athena.NET.Athena.NET.Lexer.LexicalAnalyzer.Keywords;
using Athena.NET.Athena.NET.Lexer.LexicalAnalyzer.Keywords.Literal;

namespace Athena.NET.Athena.NET.Lexer.LexicalAnalyzer
{
    internal static class KeywordsHolder
    {
        public static LiteralKeyword Digit { get; } =
            new(TokenIndentificator.Int, new LiteralOption('0', '9'));

        //This is here, just for testing
        public static LiteralKeyword Character { get; } =
            new(TokenIndentificator.Char, new LiteralOption('a', 'z'));

        //TODO: I would really like to have a
        //better storing system for reserved keywords
       
        public static ReadOnlyMemory<ReservedKeyword> ReservedKeywords =
            new ReservedKeyword[]
            {
                new (TokenIndentificator.Int, "int"),
                new (TokenIndentificator.Float, "float"),
                new (TokenIndentificator.Byte, "byte"),
                new (TokenIndentificator.Char, "char"),
                new (TokenIndentificator.If, "if"),
                new (TokenIndentificator.Else, "else"),
                new (TokenIndentificator.EqualLogical, "=="),
                new (TokenIndentificator.NotEqual, "!="),
                new (TokenIndentificator.GreaterEqual, ">="),
                new (TokenIndentificator.GreaterThan, ">"),
                new (TokenIndentificator.LessEqual, "<="),
                new (TokenIndentificator.LessThan, "<"),

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
                new (TokenIndentificator.Mul, "*", true),
                new (TokenIndentificator.Div, "/", true),

                new (TokenIndentificator.LogicalAnd, "&", true),
                new (TokenIndentificator.LogicalOr, "|", true),
                new (TokenIndentificator.LogicalXor, "^", true),
                new (TokenIndentificator.LogicalLshift, "<<", true),
                new (TokenIndentificator.LogicalRshift, ">>", true),

                new (TokenIndentificator.EqualAssignment, "=", true),
                new (TokenIndentificator.OpenBrace, "(", true),
                new (TokenIndentificator.CloseBrace, ")", true)
             };
    }
}
