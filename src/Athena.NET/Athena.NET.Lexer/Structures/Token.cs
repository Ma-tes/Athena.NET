namespace Athena.NET.Athena.NET.Lexer.Structures
{
    internal readonly struct Token
    {
        public TokenIndentificator TokenId { get; }
        public ReadOnlyMemory<char> Data { get; }

        public Token(TokenIndentificator tokenId, ReadOnlyMemory<char> data)
        {
            TokenId = tokenId;
            Data = data;
        }
    }
}
