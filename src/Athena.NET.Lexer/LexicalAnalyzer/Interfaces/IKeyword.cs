namespace Athena.NET.Lexer.LexicalAnalyzer.Interfaces
{
    internal interface IKeyword<T, TSource, TSelf> : IEquatable<TSource>
        where TSelf : IKeyword<T, TSource, TSelf>
    {
        public TokenIndentificator Identificator { get; }
        public T KeywordData { get; }

        public bool TryGetKeyword(out TSelf returnData, TSource source);
    }
}
