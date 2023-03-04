using Athena.NET.Lexer;

namespace Athena.NET.Parser.Nodes.OperatorNodes.ArithmeticOperators
{
    internal sealed class DivOperator : OperatorNode
    {
        public override OperatorPrecedence Precedence { get; }
            = OperatorPrecedence.Multiplicative;
        public override TokenIndentificator NodeToken { get; }
            = TokenIndentificator.Div;

        public DivOperator()
        {
        }

        protected override int CalculateData(int firstData, int secondData)
        {
            //TODO: Recreate this to something
            //more usable in the future
            if (secondData == 0)
                throw new Exception("Devided by zero exception");
            return firstData / secondData;
        }
    }
}
