using Athena.NET.Attributes;
using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Statements.Body;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Athena.NET.Parsing.Nodes;

public static class NodeHelper
{
    private static readonly ErrorResult<INode> unknownNodeResult =
        new ErrorResult<INode>(null!, "No valid tokens were found.");
    private static ReadOnlySpan<INode> nodeInstances => GetNodeInstances<INode>().ToArray();

    private static readonly Type tokenIdentificatorType = typeof(TokenIndentificator);
    private static readonly Type tokenTypeAttribute = typeof(TokenTypeAttribute);

    public static ResultMemory<INode> CreateNodes(this ReadOnlySpan<Token> tokens)
    {
        int tokenIndex = 0;
        var returnNodes = new ResultMemory<INode>();
        while (returnNodes.IsResultFlaw)
        {
            IResultProvider<INode> currentResultProvider = GetFirstNode(tokens[tokenIndex..]);
            INode? currentResultNode = currentResultProvider.ValueResult.Result;

            var currentResult = new ParsingResult(currentResultNode, tokenIndex);
            returnNodes.AddResult(currentResultNode is null ?
                ErrorResult<INode>.Create("No valid tokens were found.", currentResult) :
                SuccessfulResult<INode>.Create(currentResult));

            tokenIndex += currentResultProvider.ValueResult.PositionIndex;
        }
        return returnNodes;
    }

    private static IResultProvider<INode> GetFirstNode(ReadOnlySpan<Token> tokens)
    {
        int tokensLength = tokens.Length;
        for (int i = 0; i < tokensLength; i++)
        {
            if (TryGetNodeInstance(out INode result, tokens[i]))
            {
                int nodeIndex = tokens.IndexOfToken(result.NodeToken);
                result.CreateStatementResult(tokens, nodeIndex);

                return result is BodyStatement bodyStatement ?
                    bodyStatement.BodyLength + tokens.IndexOfToken(TokenIndentificator.Invoker) :
                        i + (tokens[i..].IndexOfToken(TokenIndentificator.EndLine));
            }
        }
        return ErrorResult<INode>.Create(null!, "No specific node token was found.");
    }

    //Value -1 means that wasn't found
    //any token in that span
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfTokenCondition(this ReadOnlySpan<Token> tokens, Func<Token, bool> conditionResult)
    {
        int tokensLength = tokens.Length;
        for (int i = 0; i < tokensLength; i++)
        {
            Token currentToken = tokens[i];
            bool currentResult = conditionResult.Invoke(currentToken);
            if (currentResult)
                return i;
        }
        return -1;
    }

    public static bool TryGetIndexOfToken(this ReadOnlySpan<Token> tokens,
        out int returnIndex, TokenIndentificator tokenIdentificator)
    {
        returnIndex = tokens.IndexOfToken(tokenIdentificator);
        return returnIndex != -1;
    }

    public static int IndexOfToken(this ReadOnlySpan<Token> tokens, TokenIndentificator tokenIdentificator) =>
        tokens.IndexOfTokenCondition((Token token) =>
            token.TokenId == tokenIdentificator);
    public static int IndexOfTokenType(this ReadOnlySpan<Token> tokens) =>
        tokens.IndexOfTokenCondition((Token token) =>
            token.TokenId.IsTokenType());

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

    public static INode? GetDataNode(this ReadOnlySpan<Token> tokens)
    {
        if (tokens.TryGetIndexOfToken(out int definitionCallIndex,
            TokenIndentificator.DefinitionCall))
        {
            if (TryGetNodeInstance(out INode definitionCallNode, tokens[definitionCallIndex])) 
            {
                definitionCallNode.CreateStatementResult(tokens, definitionCallIndex);
                return definitionCallNode;
            }
        }

        if (tokens.TryGetIndexOfToken(out int identifierIndex, TokenIndentificator.Identifier))
            return new IdentifierNode(tokens[identifierIndex].Data);

        int tokenTypeIndex = tokens.IndexOfTokenType();
        if (tokenTypeIndex != -1)
            return new DataNode<int>(tokens[tokenTypeIndex].TokenId, int.Parse(tokens[tokenTypeIndex].Data.Span));
        return null;
    }

    private static bool TryGetNodeInstance([NotNullWhen(true)] out INode node, Token currentToken)
    {
        int nodeInstancesLength = nodeInstances.Length;
        for (int i = 0; i < nodeInstancesLength; i++)
        {
            INode currentInstance = nodeInstances[i];
            if (currentInstance.NodeToken == currentToken.TokenId)
            {
                node = currentInstance;
                return true;
            }
        }
        return NullableHelper.NullableOutValue(out node);
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
