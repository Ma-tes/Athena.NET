using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords
{
    internal sealed class ReservedKeyword : IKeyword<ReadOnlyMemory<char>, ReadOnlyMemory<char>, ReservedKeyword>
    {
        public Func<ReadOnlyMemory<char>, char[]> ParseFunction { get; init; } = null!;
        public bool LiteralConnect { get; }

        public TokenIndentificator Identificator { get; }
        public ReadOnlyMemory<char> KeywordData { get; }

        public ReservedKeyword(TokenIndentificator id, string data, bool literalConnect = false)
        {
            Identificator = id;
            KeywordData = data.ToCharArray();
            LiteralConnect = literalConnect;
        }

        //Actually I'am not happy with this
        //solution, I hope, that I will fix
        //this as soon as possible
        public bool TryGetKeyword([NotNullWhen(true)] out ReservedKeyword returnData, ReadOnlyMemory<char> source)
        {
            if (ParseFunction is not null) 
                source = ParseFunction.Invoke(source);

            returnData = null!;
            if (IsEqual(source)) 
            {
                returnData = this;
                int keywordLength = KeywordData.Length;

                if (keywordLength == source.Length || LiteralConnect) 
                    return true;
                char nextSourceCharacter = source.Span[keywordLength];

                if ((KeywordsHolder.Character.IsEqual(nextSourceCharacter) ||
                    KeywordsHolder.Digit.IsEqual(nextSourceCharacter)) && keywordLength > 1) 
                {
                    returnData = null!;
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool IsEqual(ReadOnlyMemory<char> source) 
        {
            if (source.Length < KeywordData.Length)
                return false;

            int keywordLength = KeywordData.Length;
            var currentData = source[0..keywordLength];

            return currentData.Span.SequenceEqual(KeywordData.Span); 
        }
    }
}
