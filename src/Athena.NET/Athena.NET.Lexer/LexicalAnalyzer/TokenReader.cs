using Athena.NET.Athena.NET.Lexer.LexicalAnalyzer.Keywords;
using Athena.NET.Athena.NET.Lexer.Structures;
using System.Reflection;

namespace Athena.NET.Athena.NET.Lexer.LexicalAnalyzer
{
    internal sealed class TokenReader : LexicalTokenReader
    {
        private static readonly string tryParse = "TryParse";
        private static PrimitiveType[] primitiveTypes =
            GetPrimitiveType().ToArray();

        public ReadOnlyMemory<ReservedKeyword> ReservedKeywords { get; } =
            KeywordsHolder.ReservedKeywords;

        public TokenReader(Stream stream) : base(stream)
        {
        }

        //This is a temporary and testing solution,
        //it will be a much safer implementation
        protected override Token GetToken(ReadOnlyMemory<char> data)
        {
            var reservedKeyword = FindReservedKeyword(data);
            if (reservedKeyword != ReservedKeyword.UnknownKeyword)
                return new(reservedKeyword.Identificator, reservedKeyword.KeywordData);

            int symbolIndex = GetFirstReservedSymbolIndex(data);
            var resultData = data[0..(symbolIndex)];
            return new(GetPrimitiveToken(resultData, primitiveTypes), resultData);
        }

        //Actually I have no idea if the reflection
        //with attributes was a good choice.
        private TokenIndentificator GetPrimitiveToken(ReadOnlyMemory<char> data, ReadOnlyMemory<PrimitiveType> primitiveTypes)
        {
            string dataString = data.ToString();
            var typesSpan = primitiveTypes.Span;

            int typesLenght = primitiveTypes.Length;
            for (int i = 0; i < typesLenght; i++)
            {
                var currentType = typesSpan[i];
                Type primitiveType = currentType.Type;

                var methodInformation = primitiveType.GetMethod(tryParse, new Type[] {typeof(string), primitiveType.MakeByRefType()});
                if (methodInformation is not null) 
                {
                    bool parseResult = (bool)methodInformation.Invoke(null, new object[] { dataString, null! })!;
                    if (parseResult)
                        return currentType.TokenType;
                }
            }
            return TokenIndentificator.Identifier;
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
                KeywordsHolder.Digit.IsEqual(character))
                return false;

            var arrayHolder = new ReadOnlyMemory<char>(new[] { character });
            return FindReservedKeyword(arrayHolder) != ReservedKeyword.UnknownKeyword;
        }

        private ReservedKeyword FindReservedKeyword(ReadOnlyMemory<char> data)
        {
            var dataSpan = ReservedKeywords.Span;
            int keywordsLength = ReservedKeywords.Length;
            for (int i = 0; i < keywordsLength; i++)
            {
                var currentKeyword = dataSpan[i];
                if (currentKeyword.TryGetKeyword(out ReservedKeyword returnKeyword, data))
                    return returnKeyword;
            }
            return ReservedKeyword.UnknownKeyword;
        }

        private static IEnumerable<PrimitiveType> GetPrimitiveType()
        {
            var enumNames = Enum.GetNames<TokenIndentificator>();
            var enumValues = Enum.GetValues<TokenIndentificator>();

            int namesLength = enumNames.Length;
            for (int i = 0; i < namesLength; i++) 
            {
                var typeAttribute = typeof(TokenIndentificator).GetField(enumNames[i])!
                    .GetCustomAttribute<PrimitiveTypeAttribute>();
                if(typeAttribute is not null)
                    yield return new(enumValues[i], typeAttribute!.Type);
            }
        }
    }
}
