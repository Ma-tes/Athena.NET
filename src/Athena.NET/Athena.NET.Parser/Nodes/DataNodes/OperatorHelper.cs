using Athena.NET.Athena.NET.Lexer.Structures;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Athena.NET.Athena.NET.Parser.Nodes.DataNodes
{
    internal static class OperatorHelper
    {
        private static ReadOnlySpan<OperatorNode> operatorNodes =>
            new(GetDefaultOperators(typeof(OperatorNode)).ToArray());

        //Value -1 means that wasn't found
        //any operator token that span
        public static int IndexOfOperator(ReadOnlySpan<Token> tokens)
        {
            int returnIndex = -1;
            int tokensLength = tokens.Length;

            int lastOperatorWeight = 0;
            for (int i = 0; i < tokensLength; i++)
            {
                if (TryGetOperator(out OperatorNode currentNode, tokens[i].TokenId))
                {
                    int operatorWeight = currentNode.OperatorWeight;
                    if (operatorWeight > lastOperatorWeight) 
                    {
                        lastOperatorWeight = operatorWeight;
                        returnIndex = i;
                    }
                }
            }
            return returnIndex;
        }

        public static bool TryGetOperator([NotNullWhen(true)]out OperatorNode operatorNode, TokenIndentificator currentToken)
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
