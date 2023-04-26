using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Arithmetic;

internal sealed class AddOperator : OperatorNode
{
    public override OperatorPrecedence Precedence { get; }
        = OperatorPrecedence.Additive;
    public override TokenIndentificator NodeToken { get; }
        = TokenIndentificator.Add;

    public AddOperator()
    {
    }

    public override int CalculateData(int firstData, int secondData) =>
        firstData + secondData;
}
