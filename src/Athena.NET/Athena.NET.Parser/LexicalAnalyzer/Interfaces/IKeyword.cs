namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces
{
    internal interface IKeyword<T, TSource>
    {
        public int Identificator { get; }
        public T KeywordData { get; }

        public bool IsEqual(TSource source);
    }
}
