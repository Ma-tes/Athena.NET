namespace Athena.NET.Athena.NET.Lexer.LexicalAnalyzer.Keywords.Literal
{
    internal readonly struct LiteralOption
    {
        public char Start { get; }
        public char End { get; }

        public LiteralOption(char start, char end)
        {
            Start = start;
            End = end;
        }
    }
}
