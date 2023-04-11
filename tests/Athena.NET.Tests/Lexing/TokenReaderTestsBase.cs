using Athena.NET.Lexing;
using Athena.NET.Lexing.LexicalAnalysis;
using Athena.NET.Lexing.Structures;
using System.Text;

namespace Athena.NET.Tests.Lexing;

public class TokenReaderTestsBase
{
    protected static Token[] Tokens(params (TokenIndentificator, string)[] declarations)
    {
        return declarations
            .Select(d => new Token(d.Item1, d.Item2.AsMemory()))
            .ToArray();
    }

    protected static Token[] Tokenize(string text)
    {
        var stream = new MemoryStream();
        stream.Write(Encoding.UTF8.GetBytes(text));
        stream.Position = 0;

        using var tokenReader = new TokenReader(stream);
        return tokenReader
            .ReadTokensAsync()
            .GetAwaiter()
            .GetResult()
            .ToArray();
    }

    private protected static TokenReader CreateReader(string text)
    {
        var stream = new MemoryStream();
        stream.Write(Encoding.UTF8.GetBytes(text));
        stream.Position = 0;

        return new TokenReader(stream);
    }
}
