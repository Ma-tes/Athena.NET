using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
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

    protected override IResultProvider<INode> ExecuteParseLeftNode(ReadOnlySpan<Token> tokens)
    {
        int logicalOperatorIndex = OperatorHelper.IndexOfOperator(tokens);
        if (OperatorHelper.TryGetOperator(out OperatorNode currentNode, tokens[logicalOperatorIndex].TokenId))
        {
            if (currentNode is LogicalOperator logicalOperator)
            {
                var operatorResult = logicalOperator.CreateStatementResult(tokens, logicalOperatorIndex);
                if (operatorResult.TryGetRelativeResultProvider<INode, SuccessfulResult<INode>>(out _))
                {
                    logicalOperator.Evaluate();
                    return operatorResult;
                }
                return ErrorResult<INode>.CreateNullResult("Calculation of logical operator cannot be evaluated", logicalOperatorIndex);
            }
        }
        return ErrorResult<INode>.CreateNullResult("No logical operator was found", logicalOperatorIndex);
    }
}
