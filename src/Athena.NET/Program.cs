using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.LexicalAnalyzer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Nodes;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;

using (var tokenReader = new TokenReader
    (File.Open(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\Program.ath", FileMode.Open))) 
{
    var tokens = await tokenReader.ReadTokensAsync();

    var operatorTokenIndex = OperatorHelper.IndexOfOperator(tokens.Span);
    if (OperatorHelper.TryGetOperator(out OperatorNode resultOperator, tokens.Span[operatorTokenIndex].TokenId))
        resultOperator.CreateNodes(tokens, operatorTokenIndex);
    //WriteTokens(tokens);

    resultOperator.Evaluate();
    Console.ReadLine();
}


static void WriteTokens(ReadOnlyMemory<Token> tokens) 
{
    int tokensLength = tokens.Length;
    for (int i = 0; i < tokensLength; i++)
    {
        var currentToken = tokens.Span[i];
        if(currentToken.TokenId == TokenIndentificator.EndLine)
            Console.WriteLine($"{currentToken.TokenId} ");
        else
            Console.Write($"{currentToken.TokenId} ");

        if(currentToken.TokenId == TokenIndentificator.Identifier)
            Console.Write($"[{currentToken.Data}] ");
    }
}

