using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Logical;

internal sealed class GreaterEqualOperator : LogicalOperator
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.GreaterEqual;

    public GreaterEqualOperator()
    {
    }

    protected override bool CalculateLogicalBool(int firstValue, int secondValue) =>
        firstValue >= secondValue;
}
