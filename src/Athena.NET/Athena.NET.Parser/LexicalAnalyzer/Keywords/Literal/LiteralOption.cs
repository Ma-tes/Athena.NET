namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords.Literal
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
}
