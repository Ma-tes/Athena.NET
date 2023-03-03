using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes
{
    internal static class OperatorHelper
    {
        private static ReadOnlySpan<OperatorNode> operatorNodes =>
            new(NodeHelper.GetNodeInstances<OperatorNode>().ToArray());

        public static bool TryGetOperator([NotNullWhen(true)] out OperatorNode operatorNode, TokenIndentificator currentToken)
        {
            operatorNode = null!;
            int operatorsLength = operatorNodes.Length;
            for (int i = 0; i < operatorsLength; i++)
            {
                OperatorNode currentNode = operatorNodes[i];
                if (currentNode.NodeToken == currentToken)
                    operatorNode = currentNode;
            }

            return operatorNode is not null;
        }

        //Value -1 means that wasn't found
        //any operator token that in span
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
}
