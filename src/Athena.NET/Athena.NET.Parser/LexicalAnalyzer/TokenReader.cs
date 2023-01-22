using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Attributes;
using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Keywords;
using Athena.NET.Athena.NET.Parser.Structures;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer
{
    internal sealed class TokenReader<T> : LexicalTokenReader where T : Stream
    {
        private static ReadOnlyMemory<PrimitiveType> primitiveTypes { get; } =
            GetPrimitiveType().ToArray();
        private static readonly ReservedKeyword unknownKeyword =
            new(TokenIndentificator.Unknown, "\0u");

        public ReadOnlyMemory<ReservedKeyword> ReservedKeywords { get; } =
            KeywordsHolder.ReservedKeywords;

        public TokenReader(T stream) : base(stream)
        {
        }

        //This is temporary and testing solution,
        //it will be a much safer
        protected override Token GetToken(ReadOnlyMemory<char> data)
        {
            var reservedKeyword = FindReservedKeyword(data);
            if (reservedKeyword != unknownKeyword)
                return new(reservedKeyword.Identificator, reservedKeyword.KeywordData);

            //TODO: Create handleing for types such as
            //int, float, string and much more
            int symbolIndex = GetFirstReservedSymbolIndex(data);
            var resultData = data[0..(symbolIndex)];
            return new(GetPrimitiveToken(resultData, primitiveTypes), resultData);
        }

        //TODO: Create a better implementation
        //with reflection and without any exception
        private TokenIndentificator GetPrimitiveToken(ReadOnlyMemory<char> data, ReadOnlyMemory<PrimitiveType> primitiveTypes) 
        {
            string dataString = data.ToString();
            int typesLenght = primitiveTypes.Length;
            for (int i = 0; i < typesLenght; i++)
            {
                var currentType = primitiveTypes.Span[i];

                var typeConvertor = TypeDescriptor.GetConverter(currentType.Type);
                var convertResult = typeConvertor.ConvertFrom(dataString);
                if (convertResult is not null)
                    return currentType.TokenType;
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
            return FindReservedKeyword(arrayHolder) != unknownKeyword;
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
