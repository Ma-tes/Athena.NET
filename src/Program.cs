using Athena.NET.Athena.NET.Compiler.DataHolders;
using Athena.NET.Compiler;
using Athena.NET.Compiler.Instructions;
using Athena.NET.Lexer;
using Athena.NET.Lexer.LexicalAnalyzer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Nodes;
using System.Diagnostics.CodeAnalysis;

//This is here just for easy and fast
//debugging, it will changed as soon
//as possible
var currentRegister = new RegisterMemory<short>(OperatorCodes.AX);
currentRegister.AddData(new(0, 8), 28);
currentRegister.AddData(new(8, 4), 10);
currentRegister.AddData(new(12, 16), 1000);
currentRegister.AddData(new(12, 16), 2000);

short firstValue = currentRegister.GetData(new(0, 8));
short secondValue = currentRegister.GetData(new(8, 4));
short thirdValue = currentRegister.GetData(new(12, 16));
short quadValue = currentRegister.GetData(new(28, 16));

using (var tokenReader = new TokenReader
    (File.Open(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\ByteCodeProgram.ath", FileMode.Open)))
{
    var tokens = await tokenReader.ReadTokensAsync();
    var nodes = tokens.Span.CreateNodes();
    using (var instructionWriter = new InstructionWriter()) 
    {
        instructionWriter.CreateInstructions(nodes.Span);
        WriteInstructions(instructionWriter.InstructionList.Span);
    }

    //using (var nodeViewer = new NodeViewer(nodes, new Size(4000, 4000)))
    //{
        //Image nodeImage = nodeViewer.CreateImage();
        //nodeImage.Save(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\Node1.png", ImageFormat.Png);
        //nodeViewer.Dispose();
    //}

    //resultOperator.Evaluate();
    //WriteTokens(tokens);
}
Console.ReadLine();


static void WriteInstructions(Span<uint> instructionsSpan) 
{
    bool isInstruction = false;
    for (int i = 0; i < instructionsSpan.Length; i++)
    {
        isInstruction = instructionsSpan[i] == (uint)OperatorCodes.Nop;
        if (!isInstruction)
        {
            string instructionValue = TryGetOperatorCode(out OperatorCodes returnCode, instructionsSpan[i]) ?
                Enum.GetName(returnCode)! : $"0x{instructionsSpan[i]:X}";
            Console.Write($"{instructionValue} ");
        }
        else
            Console.WriteLine();
    }
}

static bool TryGetOperatorCode([NotNullWhen(true)]out OperatorCodes returnCode, uint data) 
{
    var enumValues = Enum.GetValues<OperatorCodes>();
    int valuesLength = enumValues.Length;
    for (int i = 0; i < valuesLength; i++)
    {
        OperatorCodes currentOperatorCode = enumValues[i];
        if ((uint)currentOperatorCode == data) 
        {
            returnCode = currentOperatorCode;
            return true;
        }
    }
    returnCode = default;
    return false;
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

