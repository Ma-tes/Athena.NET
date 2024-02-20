using Athena.NET.Compilation;
using Athena.NET.Compilation.Instructions;
using Athena.NET.Compilation.Interpreter;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing.LexicalAnalysis;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes;
using System.Diagnostics.CodeAnalysis;

//This is here for simple debugging
//and should be changed in the future
string solutionPath = Environment.CurrentDirectory[..Environment.CurrentDirectory.LastIndexOf("src")];
string examplePath = Path.Join(solutionPath, "examples", "CallProgram.ath");
Console.WriteLine(examplePath);
using (var tokenReader = new TokenReader
    (File.Open(examplePath, FileMode.Open)))
{
    var tokens = await tokenReader.ReadTokensAsync();
    var nodes = tokens.Span.CreateNodes();

    using (var instructionWriter = new InstructionWriter(GetRelativeNodes(nodes.AsSpan())))
    {
        instructionWriter.CreateInstructions(GetRelativeNodes(nodes.AsSpan()));
#if DEBUG
        WriteInstructions(instructionWriter.InstructionList.Span);
#endif
        using var virtualMachine = new VirtualMachine(instructionWriter.MainDefinitionData);
        virtualMachine.CreateInterpretation(instructionWriter.InstructionList.Span);
    }
}
Console.ReadLine();

//TODO: This is temporary solution, which is
//going to be fixed with proper exception holder.
static ReadOnlySpan<INode> GetRelativeNodes(Span<IResultProvider<INode>> resultValues)
{
    Span<INode> returnNodes = new INode[resultValues.Length];

    int valuesLength = resultValues.Length;
    for (int i = 0; i < valuesLength; i++) { returnNodes[i] = resultValues[i].ValueResult.Result; }
    return returnNodes;
}

static void WriteInstructions(Span<uint> instructionsSpan)
{
    for (int i = 0; i < instructionsSpan.Length; i++)
    {
        if (instructionsSpan[i] == (uint)OperatorCodes.Nop)
            Console.WriteLine();
        string instructionValue = TryGetOperatorCode(out OperatorCodes returnCode, instructionsSpan[i]) ?
           Enum.GetName(returnCode)! : $"0x{instructionsSpan[i]:X}";
        Console.Write($"{instructionValue} ");
    }
}

static bool TryGetOperatorCode([NotNullWhen(true)] out OperatorCodes returnCode, uint data)
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
