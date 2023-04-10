using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Logical;

internal sealed class GreaterOperator : LogicalOperator
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.GreaterThan;

    public GreaterOperator()
    {
    }

    protected override bool CalculateLogicalBool(int firstValue, int secondValue) =>
        firstValue > secondValue;
}
