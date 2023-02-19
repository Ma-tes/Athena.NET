using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.LexicalAnalyzer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes;
using Athena.NET.Athena.NET.Parser.Nodes.StatementNodes.BodyStatements;
using Athena.NET.Athena.NET.ParseViewer;
using System.Drawing;
using System.Drawing.Imaging;

//This is here just for easy and fast
//debugging, it will changed as soon
//as possible
using (var tokenReader = new TokenReader
    (File.Open(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\Program.ath", FileMode.Open))) 
{

    var tokens = await tokenReader.ReadTokensAsync();
    int assingTokenIndex = tokens.Span.IndexOfToken(TokenIndentificator.If);

    var assingTokenNode = new IfStatement();
    var result = assingTokenNode.CreateStatementResult(tokens.Span, assingTokenIndex);

    ReadOnlyMemory<INode> nodes = new INode[] { result.Node! };
    using (var nodeViewer = new NodeViewer(nodes, new Size(4000, 4000)))
    {
        Image nodeImage = nodeViewer.CreateImage();
        nodeImage.Save(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\Node1.png", ImageFormat.Png);
        nodeViewer.Dispose();
    }

    //resultOperator.Evaluate();
    //WriteTokens(tokens);
}
Console.ReadLine();


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

