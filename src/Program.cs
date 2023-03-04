﻿using Athena.NET.Lexer;
using Athena.NET.Lexer.LexicalAnalyzer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Nodes;
using Athena.NET.ParseViewer;
using System.Drawing;
using System.Drawing.Imaging;

//This is here just for easy and fast
//debugging, it will changed as soon
//as possible
using (var tokenReader = new TokenReader
    (File.Open(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\Program.ath", FileMode.Open)))
{
    var tokens = await tokenReader.ReadTokensAsync();
    var nodes = tokens.Span.CreateNodes();

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
