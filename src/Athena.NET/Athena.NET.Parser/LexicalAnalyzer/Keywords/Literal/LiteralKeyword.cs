﻿using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords.Literal
{
    internal sealed class LiteralKeyword : IKeyword<LiteralOption, char, LiteralKeyword>
    {
        public TokenIndentificator Identificator { get; }
        public LiteralOption KeywordData { get; }

        public LiteralKeyword(TokenIndentificator id, LiteralOption data)
        {
            Identificator = id;
            KeywordData = data;
        }

        public bool TryGetKeyword([NotNullWhen(true)]out LiteralKeyword returnData, char source)
        {
            bool equalSource = IsEqual(source);
            returnData = equalSource ? this : null!;

            return equalSource;
        }

        public bool IsEqual(char source) =>
            (source >= KeywordData.Start && source <= KeywordData.End);
    }
}
