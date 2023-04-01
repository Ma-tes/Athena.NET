using Athena.NET.Lexer;

namespace Athena.NET.Parser.Nodes.OperatorNodes.LogicalOperators
{
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
}
