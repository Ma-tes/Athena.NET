using Athena.NET.Athena.NET.Lexer;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes.LogicalOperators
{
    internal sealed class LessEqualOperator : LogicalOperator
    {
        public override TokenIndentificator NodeToken { get; } =
            TokenIndentificator.LessThan;

        public LessEqualOperator() 
        {
        }

        protected override bool CalculateLogicalBool(int firstValue, int secondValue) =>
            firstValue <= secondValue;
    }
}
