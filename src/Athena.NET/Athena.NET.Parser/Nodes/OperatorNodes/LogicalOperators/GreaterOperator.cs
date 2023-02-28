using Athena.NET.Athena.NET.Lexer;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes.LogicalOperators
{
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
}
