using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Logical;

internal sealed class EqualOperator : LogicalOperator
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.EqualLogical;

    public EqualOperator()
    {
    }

    protected override bool CalculateLogicalBool(int firstValue, int secondValue) =>
        firstValue == secondValue;
}
