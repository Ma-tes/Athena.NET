using Athena.NET.Lexer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.OperatorNodes.LogicalOperators;

namespace Athena.NET.Parser.Nodes.StatementNodes.BodyStatements
{
    internal sealed class IfStatement : BodyStatement
    {
        public override TokenIndentificator NodeToken { get; } =
            TokenIndentificator.If;

        protected override bool TryParseLeftNode(out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
        {
            int logicalOperatorIndex = OperatorHelper.IndexOfOperator(tokens);
            if (OperatorHelper.TryGetOperator(out OperatorNode currentNode, tokens[logicalOperatorIndex].TokenId))
            {
                if (currentNode is LogicalOperator logicalOperator) 
                {
                    nodeResult = logicalOperator.CreateStatementResult(tokens, logicalOperatorIndex);
                    if (nodeResult.ResultMessage != StatementResultMessage.Error)
                    {
                        logicalOperator.Evaluate();
                        return true;
                    }
                    return false;
                }
            }
            nodeResult = new ErrorNodeResult<INode>("No logical operator was found");
            return false;
        }
    }
}
