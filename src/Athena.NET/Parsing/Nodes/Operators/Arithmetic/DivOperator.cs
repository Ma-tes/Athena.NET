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
        //TODO: Make more flexible
        if (secondData == 0)
            throw new DivideByZeroException("Divided by zero exception");
        return firstData / secondData;
    }
}
