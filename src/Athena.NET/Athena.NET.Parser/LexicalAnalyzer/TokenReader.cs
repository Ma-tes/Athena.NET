using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords;
using Athena.NET.Athena.NET.Parser.Structures;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer
{
    internal sealed class TokenReader : LexicalTokenReader 
    {
        private static ReservedKeyword unknownKeyword =
            new(TokenIndentificator.Unknown, "\0u");

        public ReadOnlyMemory<ReservedKeyword> ReservedKeywords { get; } =
            KeywordsHolder.ReservedKeywords;
        public int TabSize { get; }

        public TokenReader(Stream stream, int tabSize = 4) : base(stream)
        {
        }

        //This is temporary and testing solution,
        //it will be a much safer
        protected override Token GetToken(ReadOnlyMemory<char> data)
        {
            var reservedKeyword = FindReservedKeyword(data);
            if (reservedKeyword == unknownKeyword)
            {
                int currentIndex = reservedKeyword.KeywordData.Length - 1;
                if (IsReservedSymbol(data.Span[(currentIndex + 1)]))
                    return new(reservedKeyword.Identificator, reservedKeyword.KeywordData);

                int indetifierIndex = GetFirstReservedSymbolIndex(data[(currentIndex + 2)..]);
                return new(TokenIndentificator.Identifier, data[0..(indetifierIndex)]);
            }

            //TODO: Create handleing for types such as
            //int, float, string and much more
            int symbolIndex = GetFirstReservedSymbolIndex(data);
            return new(TokenIndentificator.Identifier, data[0..(symbolIndex)]);
        }

        private int GetFirstReservedSymbolIndex(ReadOnlyMemory<char> data) 
        {
            int dataLength = data.Length;
            for (int i = 0; i < dataLength; i++)
            {
                char currentCharacter = data.Span[i];
                if (IsReservedSymbol(currentCharacter))
                    return i;
            }
            return (dataLength - 1);
        }

        private bool IsReservedSymbol(char character)
        {
            if (KeywordsHolder.Character.IsEqual(character) ||
                KeywordsHolder.Character.IsEqual(character))
                return false;

            var arrayHolder = new ReadOnlyMemory<char>(new[] { character });
            return FindReservedKeyword(arrayHolder) is not null;
        }

        private ReservedKeyword FindReservedKeyword(ReadOnlyMemory<char> data)
        {
            int keywordsLength = ReservedKeywords.Length;
            for (int i = 0; i < keywordsLength; i++)
            {
                var currentKeyword = ReservedKeywords.Span[i];
                if (currentKeyword.TryGetKeyword(out ReservedKeyword returnKeyword, data))
                    return returnKeyword;
            }
            return unknownKeyword;
        }
    }
}
