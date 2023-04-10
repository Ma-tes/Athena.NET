using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Arithmetic;

internal sealed class MulOperator : OperatorNode
{
    public override OperatorPrecedence Precedence { get; }
        = OperatorPrecedence.Multiplicative;
    public override TokenIndentificator NodeToken { get; }
        = TokenIndentificator.Mul;

    public MulOperator()
    {
    }

    public override int CalculateData(int firstData, int secondData) =>
        firstData * secondData;
}
