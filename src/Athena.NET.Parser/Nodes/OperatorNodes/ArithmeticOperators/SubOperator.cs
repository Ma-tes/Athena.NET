using Athena.NET.Lexer;

namespace Athena.NET.Parser.Nodes.OperatorNodes.ArithmeticOperators
{
    internal sealed class SubOperator : OperatorNode
    {
        public override OperatorPrecedence Precedence { get; }
            = OperatorPrecedence.Additive;
        public override TokenIndentificator NodeToken { get; }
            = TokenIndentificator.Sub;

        public SubOperator()
        {
        }

        public override int CalculateData(int firstData, int secondData) =>
            firstData - secondData;
    }
}
