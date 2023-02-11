using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes
{
    internal static class OperatorHelper
    {
        //This is just for testing
        private static readonly int closeBraceWeight = 100;
        private static ReadOnlySpan<OperatorNode> operatorNodes =>
            new(GetDefaultOperators(typeof(OperatorNode)).ToArray());

        public static bool TryGetOperator([NotNullWhen(true)] out OperatorNode operatorNode, TokenIndentificator currentToken)
        {
            operatorNode = null!;
            int operatorsLength = operatorNodes.Length;
            for (int i = 0; i < operatorsLength; i++)
            {
                var currentNode = operatorNodes[i];
                if (currentNode.NodeToken == currentToken)
                    operatorNode = currentNode;
            }

            return operatorNode is not null;
        }

        //Value -1 means that wasn't found
        //any operator token that in span
        public static int IndexOfOperator(ReadOnlySpan<Token> tokens)
        {
            int returnIndex = -1;
            int tokensLength = tokens.Length;

            int lastOperatorWeight = 0;
            for (int i = 0; i < tokensLength; i++)
            {
                if (tokens[i].TokenId == TokenIndentificator.OpenBrace &&
                    returnIndex != -1)
                    lastOperatorWeight += closeBraceWeight;

                if (tokens[i].TokenId == TokenIndentificator.CloseBrace)
                    lastOperatorWeight -= closeBraceWeight;

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

        private static IEnumerable<OperatorNode> GetDefaultOperators(Type assemblyType)
        {
            var currentAssembly = Assembly.GetAssembly(assemblyType);

            Type[] assemblytypes = currentAssembly!.GetTypes();
            int typesLength = assemblytypes.Length;
            for (int i = 0; i < typesLength; i++)
            {
                Type currentType = assemblytypes[i];
                if (currentType.IsSubclassOf(typeof(OperatorNode)) && !currentType.IsAbstract)
                    yield return (OperatorNode)Activator.CreateInstance(currentType)!;
            }
        }
    }
}
