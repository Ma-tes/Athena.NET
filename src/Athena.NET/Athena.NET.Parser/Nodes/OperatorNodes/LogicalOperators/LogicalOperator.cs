namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes.LogicalOperators
{
    internal abstract class LogicalOperator : OperatorNode
    {
        public sealed override OperatorPrecedence Precedence { get; } =
            OperatorPrecedence.Logical;

        protected abstract bool CalculateLogicalBool(int firstValue, int secondValue);

        protected sealed override int CalculateData(int firstData, int secondData)
        {
            //Actually, I'm really emberrased by this
            //solution, so I would like to change to
            //something more better
            return CalculateLogicalBool(firstData, secondData) ? 1 : 0;
        }
    }
}
