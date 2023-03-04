using Athena.NET.Lexer;

namespace Athena.NET.Parser.Nodes.OperatorNodes.LogicalOperators
{
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
}
