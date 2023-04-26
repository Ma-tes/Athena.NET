using Athena.NET.Lexing.Structures;
using System.Text;

namespace Athena.NET.Lexing;

internal abstract class LexicalTokenReader : IDisposable
{
    private static readonly Encoding defaultEncoding =
        Encoding.UTF8;
    private StreamReader streamReader;

    public Memory<char> ReaderData { get; }
    public long ReaderLength { get; }
    public int ReaderPosition { get; private set; }

    public LexicalTokenReader(Stream stream)
    {
        streamReader = new(stream, defaultEncoding, false);

        ReaderLength = streamReader.BaseStream.Length;
        ReaderData = new char[ReaderLength];
    }

    public async Task<ReadOnlyMemory<Token>> ReadTokensAsync()
    {
        var returnTokens = new List<Token>();

        await streamReader.ReadAsync(ReaderData);
        while (ReaderPosition < ReaderLength)
        {
            Memory<char> currentData = ReaderData[ReaderPosition..];
            Token currentToken = GetToken(currentData);

            returnTokens.Add(currentToken);
            ReaderPosition += currentToken.Data.Length;
        }
        return returnTokens.ToArray();
    }

    protected abstract Token GetToken(ReadOnlyMemory<char> data);

    public void Dispose()
    {
        streamReader.Dispose();
        GC.SuppressFinalize(this);
    }
}
