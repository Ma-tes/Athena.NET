namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces
{
    internal interface IKeyword<T, TSource, TSelf> where TSelf : IKeyword<T, TSource, TSelf>
    {
        public TokenIndentificator Identificator { get; }
        public T KeywordData { get; }

        public bool IsEqual(TSource source);
        public bool TryGetKeyword(out TSelf returnData, TSource source);
    }
}
