namespace Athena.NET.Lexer.Structures
{
    public readonly struct Token
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
