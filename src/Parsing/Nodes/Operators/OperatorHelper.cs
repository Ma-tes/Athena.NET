using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Operators;

internal static class OperatorHelper
{
    private static ReadOnlySpan<OperatorNode> operatorNodes =>
        new(NodeHelper.GetNodeInstances<OperatorNode>().ToArray());

    public static bool TryGetOperator([NotNullWhen(true)] out OperatorNode operatorNode, TokenIndentificator currentToken)
    {
        int operatorsLength = operatorNodes.Length;
        for (int i = 0; i < operatorsLength; i++)
        {
            OperatorNode currentNode = operatorNodes[i];
            if (currentNode.NodeToken == currentToken) 
            {
                operatorNode = currentNode;
                return true;
            }
        }
        return NullableHelper.NullableOutValue(out operatorNode);
    }

    public static bool TryGetOperatorResult(out NodeResult<INode> operatorResult, ReadOnlySpan<Token> tokens)
    {
        int operatorIndex = IndexOfOperator(tokens);
        if (operatorIndex != -1 && TryGetOperator(out OperatorNode operatorNode, tokens[operatorIndex].TokenId))
        {
            operatorResult = operatorNode.CreateStatementResult(tokens, operatorIndex);
            return operatorResult.ResultMessage != StatementResultMessage.Error;
        }
        operatorResult = new ErrorNodeResult<INode>("Any valid operator node wasn't found");
        return false;
    }

    //If no operator is found, returns -1
    public static int IndexOfOperator(ReadOnlySpan<Token> tokens)
    {
        int lastOperatorWeight = 0;
        int returnIndex = -1;

        int tokensLength = tokens.Length;
        for (int i = 0; i < tokensLength; i++)
        {
            if (tokens[i].TokenId == TokenIndentificator.OpenBrace &&
                returnIndex != -1)
                lastOperatorWeight += (int)OperatorPrecedence.Brace;
            if (tokens[i].TokenId == TokenIndentificator.CloseBrace)
                lastOperatorWeight -= (int)OperatorPrecedence.Brace;

            if (TryGetOperator(out OperatorNode currentNode, tokens[i].TokenId))
            {
                int operatorWeight = (int)currentNode.Precedence;
                if (operatorWeight >= lastOperatorWeight)
                {
                    lastOperatorWeight = operatorWeight;
                    returnIndex = i;
                }
            }
        }
        return returnIndex;
    }
}
