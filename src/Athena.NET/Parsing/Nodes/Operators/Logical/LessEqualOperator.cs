using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Logical;

internal sealed class LessEqualOperator : LogicalOperator
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.LessEqual;

    public LessEqualOperator()
    {
    }

    protected override bool CalculateLogicalBool(int firstValue, int secondValue) =>
        firstValue <= secondValue;
}
