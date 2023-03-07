using Athena.NET.Lexer;

namespace Athena.NET.Parser.Nodes.OperatorNodes.ArithmeticOperators
{
    internal sealed class AddOperator : OperatorNode
    {
        public override OperatorPrecedence Precedence { get; }
            = OperatorPrecedence.Additive;
        public override TokenIndentificator NodeToken { get; }
            = TokenIndentificator.Add;

        public AddOperator()
        {
        }

        public override int CalculateData(int firstData, int secondData) =>
            firstData + secondData;
    }
}
