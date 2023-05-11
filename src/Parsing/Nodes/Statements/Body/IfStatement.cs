using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.Parsing.Nodes.Operators.Logical;

namespace Athena.NET.Parsing.Nodes.Statements.Body;

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
                nodeResult = new ErrorNodeResult<INode>("Calculation of logical operator cannot be evaluated");
                return false;
            }
        }
        nodeResult = new ErrorNodeResult<INode>("No logical operator was found");
        return false;
    }
}
