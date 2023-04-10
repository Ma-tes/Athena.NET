using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Arithmetic;

internal sealed class SubOperator : OperatorNode
{
    public override OperatorPrecedence Precedence { get; }
        = OperatorPrecedence.Additive;
    public override TokenIndentificator NodeToken { get; }
        = TokenIndentificator.Sub;

    public SubOperator()
    {
    }

    public override int CalculateData(int firstData, int secondData) =>
        firstData - secondData;
}
