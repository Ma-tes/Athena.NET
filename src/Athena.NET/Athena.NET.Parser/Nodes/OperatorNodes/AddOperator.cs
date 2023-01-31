using Athena.NET.Athena.NET.Lexer.Structures;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes
{
    internal sealed class AddOperator : OperatorNode
    {
        public override int OperatorWeight { get; } = 4;
        public override TokenIndentificator NodeToken { get; } = TokenIndentificator.Add;

        public AddOperator(ReadOnlyMemory<Token> tokens, int nodeIndex) : base(tokens, nodeIndex) 
        {

        }

        protected override int CalculateData(int firstData, int secondData) =>
            firstData + secondData;
    }
}
