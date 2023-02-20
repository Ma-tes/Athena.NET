using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Athena.NET.Parser.Nodes.StatementNodes.BodyStatements;
using Athena.NET.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Athena.NET.Athena.NET.Parser.Nodes
{
    public static class NodeHelper
    {
        private static ReadOnlySpan<INode> nodeInstances =>
            GetNodeInstances<INode>().ToArray();

        private static readonly Type tokenIdentificatorType =
            typeof(TokenIndentificator);
        private static readonly Type tokenTypeAttribute =
            typeof(TokenTypeAttribute);


        //Actually I'm not entiry sure
        //about the name of this method
        //so it's possible, that I will
        //change it to something else
        public static ReadOnlyMemory<INode> CreateNodes(this ReadOnlySpan<Token> tokens) 
        {
            int tokenIndex = 0;
            var returnNodes = new List<INode>();

            int currentNodeSize = GetFirstNode(out INode currentNode, tokens[tokenIndex..]);
            while(currentNodeSize != -1)
            {
                returnNodes.Add(currentNode);
                tokenIndex += currentNodeSize;

                currentNodeSize = GetFirstNode(out INode _, tokens[tokenIndex..]);
            }
            return returnNodes.ToArray();
        }

        private static int GetFirstNode([NotNullWhen(true)]out INode nodeResult, ReadOnlySpan<Token> tokens) 
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                if (TryGetNodeInstance(out INode result, tokens[i]))
                {
                    int nodeIndex = tokens.IndexOfToken(result.NodeToken);
                    result.CreateStatementResult(tokens, nodeIndex);

                    nodeResult = result;
                    return result is BodyStatement bodyStatement ?
                        bodyStatement.BodyLength : 
                        i + (tokens[i..].IndexOfToken(TokenIndentificator.Semicolon));
                }
            }
            nodeResult = null!;
            return -1;
        }

        public static bool TryGetIndexOfToken(this ReadOnlySpan<Token> tokens, out int index, TokenIndentificator tokenIdentificator) 
        {
            index = tokens.IndexOfToken(tokenIdentificator);
            return index != -1;
        }

        //Value -1 means that wasn't found
        //any token in that span
        public static int IndexOfToken(this ReadOnlySpan<Token> tokens, TokenIndentificator tokenIdentificator)
        {
            int tokensLength = tokens.Length;
            for (int i = 0; i < tokensLength; i++)
            {
                if (tokens[i].TokenId == tokenIdentificator)
                    return i;
            }
            return -1;
        }

        //Value -1 means that wasn't found
        //any token in that span
        public static int IndexOfTokenType(this ReadOnlySpan<Token> tokens) 
        {
            int tokensLength = tokens.Length;
            for (int i = 0; i < tokensLength; i++)
            {
                TokenIndentificator currentIdentificator = tokens[i].TokenId;
                if (currentIdentificator.IsTokenType())
                    return i;
            }
            return -1;
        }

        public static IEnumerable<T> GetNodeInstances<T>() where T : INode
        {
            Type parentNodeType = typeof(T);
            var currentAssembly = Assembly.GetAssembly(parentNodeType);

            Type[] assemblytypes = currentAssembly!.GetTypes();
            int typesLength = assemblytypes.Length;
            for (int i = 0; i < typesLength; i++)
            {
                Type currentType = assemblytypes[i];
                if ((currentType.IsAssignableTo(parentNodeType) && !currentType.IsAbstract) 
                    && !IsDataNode(currentType))
                    yield return (T)Activator.CreateInstance(currentType)!;
            }
        }

        private static bool TryGetNodeInstance([NotNullWhen(true)]out INode node, Token currentToken) 
        {
            int nodeInstancesLength = nodeInstances.Length;
            for (int i = 0; i < nodeInstancesLength; i++)
            {
                var currentInstance = nodeInstances[i];
                if (currentInstance.NodeToken == currentToken.TokenId) 
                {
                    node = currentInstance;
                    return true;
                }
            }

            node = null!;
            return false;
        }

        private static bool IsDataNode(Type nodeType) 
        {
            Type dataType = typeof(DataNode<>);
            if (nodeType == dataType)
                return true;
            return nodeType.BaseType!.IsGenericType 
                && nodeType.BaseType!.GetGenericTypeDefinition() == dataType;
        }

        private static bool IsTokenType(this TokenIndentificator tokenIndentificator) 
        {
            string tokenMemberName = tokenIndentificator.ToString();
            var memberInformations = tokenIdentificatorType.GetMember(tokenMemberName);
            if (memberInformations.Length == 0)
                throw new Exception($"Member:{tokenMemberName} in TokenIndetificator wasn't found");

            var tokenAttribute = memberInformations[0].GetCustomAttribute(tokenTypeAttribute);
            return tokenAttribute is not null;
        }
    }
}
