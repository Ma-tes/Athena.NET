using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Attributes;
using System.Reflection;

namespace Athena.NET.Athena.NET.Parser.Nodes
{
    public static class NodeHelper
    {
        private static readonly Type tokenIdentificatorType =
            typeof(TokenIndentificator);
        private static readonly Type tokenTypeAttribute =
            typeof(TokenTypeAttribute);

        //Value -1 means that wasn't found
        //any token in that span
        public static int IndexOfToken(this ReadOnlySpan<Token> tokens, TokenIndentificator tokenIdentificator)
        {
            int tokensLength = tokens.Length;
            for (int i = 0; i < tokensLength; i++)
            {
                if (tokens[i].TokenId == tokenIdentificator)
                    return i;
            }
            return -1;
        }

        //Value -1 means that wasn't found
        //any token in that span
        public static int IndexOfTokenType(this ReadOnlySpan<Token> tokens) 
        {
            int tokensLength = tokens.Length;
            for (int i = 0; i < tokensLength; i++)
            {
                TokenIndentificator currentIdentificator = tokens[i].TokenId;
                if (currentIdentificator.IsTokenType())
                    return i;
            }
            return -1;
        }

        private static bool IsTokenType(this TokenIndentificator tokenIndentificator) 
        {
            string tokenMemberName = tokenIndentificator.ToString();
            var memberInformations = tokenIdentificatorType.GetMember(tokenMemberName);
            if (memberInformations.Length == 0)
                throw new Exception($"Member:{tokenMemberName} in TokenIndetificator wasn't found");

            var tokenAttribute = memberInformations[0].GetCustomAttribute(tokenTypeAttribute);
            return tokenAttribute is not null;
        }
    }
}
