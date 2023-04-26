using Athena.NET.Lexing;

namespace Athena.NET.Parsing.Nodes.Operators.Logical;

internal sealed class LessOperator : LogicalOperator
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.LessThan;

    public LessOperator()
    {
    }

    protected override bool CalculateLogicalBool(int firstValue, int secondValue) =>
        firstValue < secondValue;
}
