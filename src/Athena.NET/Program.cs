using Athena.NET.Athena.NET.Parser.LexicalAnalyzer;
using Athena.NET.Athena.NET.Parser.Structures;

using (var tokenReader = new TokenReader<FileStream>
    (File.Open(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\Program.ath", FileMode.Open))) 
{
    var tokens = await tokenReader.ReadLexicalTokensAsync();
    WriteTokens(tokens);
    Console.ReadLine();
}

static void WriteTokens(ReadOnlyMemory<Token> tokens) 
{
    int tokensLength = tokens.Length;
    for (int i = 0; i < tokensLength; i++)
    {
        var currentToken = tokens.Span[i];
        if(currentToken.TokenId == Athena.NET.Athena.NET.Parser.TokenIndentificator.EndLine)
            Console.WriteLine($"{currentToken.TokenId} ");
        else
            Console.Write($"{currentToken.TokenId} ");

        if(currentToken.TokenId == Athena.NET.Athena.NET.Parser.TokenIndentificator.Identifier)
            Console.Write($"[{currentToken.Data}] ");
    }
}

