using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Arithmetic;

internal sealed class DivOperator : OperatorNode
{
    public override OperatorPrecedence Precedence { get; }
        = OperatorPrecedence.Multiplicative;
    public override TokenIndentificator NodeToken { get; }
        = TokenIndentificator.Div;

    public DivOperator()
    {
    }

    public override int CalculateData(int firstData, int secondData)
    {
        //TODO: Recreate this to something
        //more usable in the future
        if (secondData == 0)
            throw new DivideByZeroException("Divided by zero exception");
        return firstData / secondData;
    }
}
