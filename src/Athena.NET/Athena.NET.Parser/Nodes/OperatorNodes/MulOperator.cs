using Athena.NET.Athena.NET.Lexer.Structures;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes
{
    internal sealed class MulOperator : OperatorNode
    {
        public override OperatorPrecedence Precedence { get; } = OperatorPrecedence.Multiplicative;
        public override TokenIndentificator NodeToken { get; } = TokenIndentificator.Mul;

        public MulOperator()
        {
        }

        protected override int CalculateData(int firstData, int secondData) =>
            firstData * secondData;
    }
}
