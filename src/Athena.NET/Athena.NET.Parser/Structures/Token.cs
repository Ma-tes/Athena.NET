namespace Athena.NET.Athena.NET.Parser.Structures
{
    internal readonly struct Token
    {
        public TokenIndentificator TokenId { get; }
        public ReadOnlyMemory<byte> Data { get; }

        public Token(TokenIndentificator tokenId, ReadOnlyMemory<byte> data) 
        {
            TokenId = tokenId;
            Data = data;
        }
    }
}
