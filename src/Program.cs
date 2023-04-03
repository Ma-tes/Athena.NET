using Athena.NET.Compiler;
using Athena.NET.Compiler.Instructions;
using Athena.NET.Compiler.Interpreter;
using Athena.NET.Compiler.Structures;
using Athena.NET.Lexer;
using Athena.NET.Lexer.LexicalAnalyzer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Nodes;
using System.Diagnostics.CodeAnalysis;

//This is here just for easy and fast
//debugging, it will changed as soon
//as possible
using (var tokenReader = new TokenReader
    (File.Open(@"C:\Users\uzivatel\source\repos\Athena.NET\examples\StoreInstructionsProgram.ath", FileMode.Open)))
{
    var tokens = await tokenReader.ReadTokensAsync();
    var nodes = tokens.Span.CreateNodes();

    using (var instructionWriter = new InstructionWriter()) 
    {
        instructionWriter.CreateInstructions(nodes.Span);
        WriteInstructions(instructionWriter.InstructionList.Span);
        using (var virtualMachine = new VirtualMachine()) 
        {
            virtualMachine.CreateInterpretation(instructionWriter.InstructionList.Span);
            if (virtualMachine.TryGetRegisterMemory(out RegisterMemory? axRegister, OperatorCodes.AX))
                WriteRegisterData(axRegister,
                    new RegisterData(16, 8),
                    new RegisterData(32, 8)
                    );
        }
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


static void WriteRegisterData(RegisterMemory memory, params RegisterData[] registerData) 
{
    int dataLength = registerData.Length;
    for (int i = 0; i < registerData.Length; i++)
    {
        RegisterData currentRegister = registerData[i];
        int currentData = (int)memory.GetData(currentRegister);
        Console.Write($"\n[{memory.RegisterCode}]: ({currentRegister.Size}, {currentRegister.Offset}) = {currentData}");
    }
}

static void WriteInstructions(Span<uint> instructionsSpan) 
{
    bool isInstruction = false;
    for (int i = 0; i < instructionsSpan.Length; i++)
    {
        isInstruction = instructionsSpan[i] == (uint)OperatorCodes.Nop;
        if (isInstruction)
            Console.WriteLine();
         string instructionValue = TryGetOperatorCode(out OperatorCodes returnCode, instructionsSpan[i]) ?
            Enum.GetName(returnCode)! : $"0x{instructionsSpan[i]:X}";
         Console.Write($"{instructionValue} ");
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

