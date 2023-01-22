﻿using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces;
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
        //it as soon as possible
        public bool TryGetKeyword([NotNullWhen(true)] out ReservedKeyword returnData, ReadOnlyMemory<char> source)
        {
            if (ParseFunction is not null) 
                source = ParseFunction.Invoke(source);

            bool keywordIsEqual = IsEqual(source);
            returnData = keywordIsEqual ? this : null!;

            int keywordLength = KeywordData.Length;
            if (!keywordIsEqual || (keywordLength == source.Length || LiteralConnect))
                return keywordIsEqual;

            char nextSourceCharacter = source.Span[keywordLength];
            if ((KeywordsHolder.Character.IsEqual(nextSourceCharacter) ||
                KeywordsHolder.Digit.IsEqual(nextSourceCharacter)) && keywordLength > 1)
            {
                returnData = null!;
                return false;
            }
            return true;
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
